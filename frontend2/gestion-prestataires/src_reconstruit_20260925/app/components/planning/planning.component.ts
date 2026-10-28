import { Component, OnInit, OnDestroy, ChangeDetectorRef, HostListener, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlanningService } from '../../services/planning.service';
import { PrestataireService } from '../../services/prestataire.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';
import { ApplicationPlanning, ApplicationServerDto, ServerType } from '../../models/planning.model';
import { Prestataire } from '../../models/prestataire.model';
import { SearchPeoplePipe } from '../../shared/search-people.pipe';
import { environment } from '../../../environments/environment';

type ViewMode = 'week' | 'month' | 'year';

interface TimelineUnit { date: Date; label: string; isToday: boolean; }
interface TimelineGroup { label: string; unitCount: number; }
interface BarPosition { left: number; width: number; }

const ROW_HEIGHT = 80;
const CELL_W: Record<ViewMode, number> = { week: 40, month: 40, year: 60 };

@Component({
  selector: 'app-planning',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationToastComponent, SearchPeoplePipe],
  templateUrl: './planning.component.html',
  styleUrls: ['./planning.component.scss']
})
export class PlanningComponent implements OnInit {

  @ViewChild('ganttScroll') ganttScrollRef?: ElementRef<HTMLDivElement>;

  // ── Données ────────────────────────────────────────────────────────────────
  applications: ApplicationPlanning[] = [];
  prestataires: Prestataire[] = [];
  responsables: Prestataire[] = [];
  //   Toggle "Inclure désactivés" : filtres + dropdowns du modal
  showInactivePrest = false;
  isLoading = false;

  // ── Timeline ───────────────────────────────────────────────────────────────
  viewMode: ViewMode = 'year';
  referenceDate = new Date();
  readonly ROW_HEIGHT = ROW_HEIGHT;
  imageUrl = '';

  units: TimelineUnit[] = [];
  groups: TimelineGroup[] = [];
  cellWidth = CELL_W.year;
  totalWidth = 0;

  // ── Filtre ─────────────────────────────────────────────────────────────────
  showPrestFilter = false;
  filterPrestId: string | null = null;

  // ── Modal ──────────────────────────────────────────────────────────────────
  showModal = false;
  modalMode: 'create' | 'edit' = 'create';
  editingApp: ApplicationPlanning | null = null;

  formNom = '';
  formDescription = '';
  formDateDebut = '';
  formDateFin = '';
  formStatut = 'Planifié';
  formPriorite = 'Moyenne';
  formCouleur = '#3b82f6';
  formCouleurCustom = '';
  formAvancement = 0;
  formPrestataires: string[] = [];
  formResponsables: string[] = [];
  dateErreur = '';

  //   Serveurs / environnements de l'application
  formServers: ApplicationServerDto[] = [];
  readonly SERVER_TYPES: ServerType[] = ['DEV', 'RECETTE', 'PROD', 'INT', 'AUTRE'];

  //   Libellés affichés dans le select (côté UI uniquement, backend reçoit la valeur brute)
  readonly SERVER_TYPE_LABELS: Record<ServerType, string> = {
    DEV: 'DEV',
    RECETTE: 'RECETTE',
    PROD: 'PROD',
    INT: 'INTÉGRATION',
    AUTRE: 'Autre'
  };

  showDdPrest = false;
  showDdResp  = false;
  searchPrest = '';
  searchResp  = '';

  //   Onglets du modal — évite un long scroll
  activeModalTab: 'general' | 'equipe' | 'serveurs' = 'general';
  setModalTab(t: 'general' | 'equipe' | 'serveurs'): void {
    this.activeModalTab = t;
    //   Ferme les dropdowns ouverts en changeant d'onglet
    this.showDdPrest = false;
    this.showDdResp = false;
  }

  // ── Suppression ────────────────────────────────────────────────────────────
  showDeleteConfirm = false;
  appToDelete: ApplicationPlanning | null = null;

  // ── Tooltip ────────────────────────────────────────────────────────────────
  tooltip: { show: boolean; app: ApplicationPlanning | null; x: number; y: number } = { show: false, app: null, x: 0, y: 0 };

  // ── Drag & drop (ordre des lignes) ─────────────────────────────────────────
  dragFromIndex: number | null = null;
  dragOverIndex: number | null = null;
  orderedApps: ApplicationPlanning[] = [];

  readonly COULEURS_PRESET = [
    '#3b82f6', '#6366f1', '#8b5cf6', '#a855f7', '#ec4899', '#ef4444', '#f97316',
    '#f59e0b', '#eab308', '#84cc16', '#22c55e', '#10b981', '#14b8a6', '#06b6d4'
  ];
  readonly STATUTS = ['Planifié', 'En cours', 'Terminé', 'En pause'];
  readonly PRIORITES = ['Faible', 'Moyenne', 'Haute', 'Critique'];

  constructor(
    private planningService: PlanningService,
    private prestataireService: PrestataireService,
    private notifService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadPrestataires();
    this.loadApplications();
    this.buildTimeline();
  }

  // ── Chargement ─────────────────────────────────────────────────────────────
  loadApplications(): void {
    this.isLoading = true;
    this.planningService.getAll().subscribe({
      next: data => {
        this.applications = data;
        this.orderedApps = [...data].sort((a, b) => {
          const oa = (a as any).ordre ?? 9999;
          const ob = (b as any).ordre ?? 9999;
          return oa - ob;
        });
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.notifService.show('error', 'Erreur chargement'); this.isLoading = false; }
    });
  }

  loadPrestataires(): void {
    this.prestataireService.getAll(this.showInactivePrest).subscribe({
      next: d => {
        this.responsables  = d.filter(p => p.responsable === true);
        this.prestataires  = d.filter(p => !p.responsable);
        this.cdr.detectChanges();
      }
    });
  }

  //   Bascule l'inclusion des désactivés (rechargement)
  toggleShowInactivePrest(): void {
    this.showInactivePrest = !this.showInactivePrest;
    //   Nettoie le filtre par prestataire si pointé sur un inactif retiré
    if (!this.showInactivePrest && this.filterPrestId) {
      const sel = [...this.prestataires, ...this.responsables].find(p => p.id === this.filterPrestId);
      if (sel?.isActive === false) this.filterPrestId = null;
    }
    this.loadPrestataires();
  }

  get filteredApps(): ApplicationPlanning[] {
    const list = this.orderedApps.length ? this.orderedApps : this.applications;
    if (!this.filterPrestId) return list;
    return list.filter(a => a.prestataires.some(p => p.prestataireId === this.filterPrestId));
  }

  // ── Timeline ───────────────────────────────────────────────────────────────
  buildTimeline(): void {
    this.cellWidth = CELL_W[this.viewMode];
    this.units = [];
    this.groups = [];
    const today = new Date();

    if (this.viewMode === 'year') {
      // Vue Année : 1 colonne = 1 mois, de N-1 à N+3
      const startYear = this.referenceDate.getFullYear() - 1;
      const endYear = startYear + 4;
      const groupMap = new Map<string, number>();
      for (let y = startYear; y <= endYear; y++) {
        for (let m = 0; m < 12; m++) {
          const d = new Date(y, m, 1);
          const key = `${y}`;
          groupMap.set(key, (groupMap.get(key) || 0) + 1);
          this.units.push({
            date: d,
            label: d.toLocaleDateString('fr-FR', { month: 'short' }),
            isToday: today.getFullYear() === y && today.getMonth() === m
          });
        }
      }
      this.groups = Array.from(groupMap.entries()).map(([label, unitCount]) => ({ label, unitCount }));

    } else if (this.viewMode === 'month') {
      // Vue Mois : 1 colonne = 1 semaine ISO de l'année de référence
      const year = this.referenceDate.getFullYear();
      let cursor = this.getMonday(new Date(year, 0, 4));
      const end = new Date(year, 11, 31);
      const groupMap = new Map<string, number>();
      while (cursor.getFullYear() <= year && cursor <= end) {
        const week = this.isoWeek(cursor);
        const mid = new Date(cursor);
        mid.setDate(cursor.getDate() + 3);
        const key = mid.toLocaleDateString('fr-FR', { month: 'short' });
        groupMap.set(key, (groupMap.get(key) || 0) + 1);
        this.units.push({ date: new Date(cursor), label: `S${week}`, isToday: this.sameWeek(today, cursor) });
        cursor = new Date(cursor);
        cursor.setDate(cursor.getDate() + 7);
        if (cursor.getFullYear() > year) break;
      }
      this.groups = Array.from(groupMap.entries()).map(([label, unitCount]) => ({ label, unitCount }));

    } else {
      // Vue Semaine : 1 colonne = 1 jour, au moins 6 semaines et toutes les applications
      const start = this.getMonday(this.referenceDate);
      let end = new Date(start);
      end.setDate(start.getDate() + 41);
      for (const app of this.applications) {
        const f = new Date(app.dateFin);
        if (f > end) end = f;
      }
      let minStart = new Date(start);
      for (const app of this.applications) {
        const d = new Date(app.dateDebut);
        if (d < minStart) minStart = d;
      }
      const realStart = this.getMonday(minStart);
      const groupMap = new Map<string, number>();
      let cursor = new Date(realStart);
      while (cursor <= end) {
        const key = cursor.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' });
        groupMap.set(key, (groupMap.get(key) || 0) + 1);
        this.units.push({
          date: new Date(cursor),
          label: cursor.toLocaleDateString('fr-FR', { weekday: 'short', day: 'numeric' }),
          isToday: this.sameDay(today, cursor)
        });
        cursor = new Date(cursor);
        cursor.setDate(cursor.getDate() + 1);
      }
      this.groups = Array.from(groupMap.entries()).map(([label, unitCount]) => ({ label, unitCount }));
    }

    this.totalWidth = this.units.length * this.cellWidth;
    setTimeout(() => this.scrollToToday(), 50);
  }

  scrollToToday(): void {
    if (!this.ganttScrollRef) return;
    const px = this.getTodayPx();
    if (px !== null) this.ganttScrollRef.nativeElement.scrollLeft = Math.max(0, px - 300);
  }

  previous(): void {
    if (this.viewMode === 'year') this.referenceDate = new Date(this.referenceDate.getFullYear() - 1, 6, 1);
    else if (this.viewMode === 'month') this.referenceDate = new Date(this.referenceDate.getFullYear() - 1, 0, 1);
    else { const d = new Date(this.referenceDate); d.setDate(d.getDate() - 42); this.referenceDate = d; }
    this.buildTimeline();
  }

  next(): void {
    if (this.viewMode === 'year') this.referenceDate = new Date(this.referenceDate.getFullYear() + 1, 6, 1);
    else if (this.viewMode === 'month') this.referenceDate = new Date(this.referenceDate.getFullYear() + 1, 0, 1);
    else { const d = new Date(this.referenceDate); d.setDate(d.getDate() + 42); this.referenceDate = d; }
    this.buildTimeline();
  }

  goToday(): void { this.referenceDate = new Date(); this.buildTimeline(); }
  setView(v: ViewMode): void { this.viewMode = v; this.buildTimeline(); }

  get periodLabel(): string {
    if (this.viewMode === 'year') return `${this.referenceDate.getFullYear() - 2} – ${this.referenceDate.getFullYear() + 2}`;
    if (this.viewMode === 'month') return `${this.referenceDate.getFullYear()}`;
    return this.referenceDate.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' });
  }

  // ── Positionnement des barres ──────────────────────────────────────────────
  getBarPosition(app: ApplicationPlanning): BarPosition | null {
    if (!this.units.length) return null;
    const appStart = new Date(app.dateDebut).getTime();
    const appEnd = new Date(app.dateFin).getTime();
    const unitMs = this.viewMode === 'week' ? 86400000 : this.viewMode === 'year' ? 30.4375 * 86400000 : 7 * 86400000;
    const tlStart = this.units[0].date.getTime();
    const tlEnd = this.units[this.units.length - 1].date.getTime() + unitMs;
    if (appEnd <= tlStart || appStart >= tlEnd) return null;
    const cs = Math.max(appStart, tlStart);
    const ce = Math.min(appEnd, tlEnd);
    const totalMs = this.units.length * unitMs;
    const left = ((cs - tlStart) / totalMs) * this.totalWidth;
    const width = Math.max(((ce - cs) / totalMs) * this.totalWidth, 4);
    return { left: Math.round(left), width: Math.round(width) };
  }

  getTodayPx(): number | null {
    if (!this.units.length) return null;
    const now = Date.now();
    const unitMs = this.viewMode === 'week' ? 86400000 : this.viewMode === 'year' ? 30.4375 * 86400000 : 7 * 86400000;
    const tlStart = this.units[0].date.getTime();
    const totalMs = this.units.length * unitMs;
    const px = ((now - tlStart) / totalMs) * this.totalWidth;
    return px < 0 || px > this.totalWidth ? null : Math.round(px);
  }

  get todayLineStyle(): Record<string, string> {
    const px = this.getTodayPx();
    return px === null ? { display: 'none' } : { left: `${px}px` };
  }

  get totalHeightPx(): number { return this.filteredApps.length * ROW_HEIGHT; }
  getRowTop(i: number): number { return i * ROW_HEIGHT; }

  darkenColor(hex: string, amount: number): string {
    const h = hex.replace('#', '');
    if (h.length !== 6) return hex;
    const n = parseInt(h, 16);
    const r = Math.max(0, (n >> 16) - amount);
    const g = Math.max(0, ((n >> 8) & 0xff) - amount);
    const b = Math.max(0, (n & 0xff) - amount);
    return `#${((r << 16) | (g << 8) | b).toString(16).padStart(6, '0')}`;
  }

  // ── Modal ─────────────────────────────────────────────────────────────────
  openCreate(): void { this.modalMode='create'; this.editingApp=null; this.resetForm(); this.activeModalTab='general'; this.showModal=true; }

  openEdit(app: ApplicationPlanning): void {
    this.modalMode='edit'; this.editingApp=app;
    this.formNom=app.nom; this.formDescription=app.description||'';
    this.formDateDebut=app.dateDebut.substring(0,10); this.formDateFin=app.dateFin.substring(0,10);
    this.formStatut=app.statut; this.formPriorite=app.priorite;
    this.formCouleur=app.couleur; this.formAvancement=app.avancement;
    const prestIds = new Set(this.prestataires.map(p=>p.id!));
    this.formPrestataires = app.prestataires.filter(p=>prestIds.has(p.prestataireId)).map(p=>p.prestataireId);
    this.formResponsables  = app.prestataires.filter(p=>!prestIds.has(p.prestataireId)).map(p=>p.prestataireId);
    //   Clone profond pour permettre l'annulation
    this.formServers = (app.servers ?? []).map(s => ({
      id: s.id,
      type: s.type,
      identifiant: s.identifiant ?? '',
      libelle: s.libelle ?? '',
      note: s.note ?? '',
      ordre: s.ordre ?? 0
    }));
    this.dateErreur=''; this.activeModalTab='general'; this.showModal=true;
  }

  closeModal(): void {
    this.showModal=false; this.editingApp=null; this.dateErreur='';
    this.showDdPrest=false; this.showDdResp=false; this.searchPrest=''; this.searchResp='';
  }

  resetForm(): void {
    this.formNom=''; this.formDescription=''; this.formDateDebut=''; this.formDateFin='';
    this.formStatut='Planifié'; this.formPriorite='Moyenne'; this.formCouleur='#3b82f6';
    this.formCouleurCustom=''; this.formAvancement=0;
    this.formPrestataires=[]; this.formResponsables=[]; this.dateErreur='';
    this.showDdPrest=false; this.showDdResp=false; this.searchPrest=''; this.searchResp='';
    this.formServers=[];
  }

  // ── Serveurs / Environnements ─────────────────────────────────────────────
  addServer(type: ServerType = 'DEV'): void {
    this.formServers.push({
      type,
      identifiant: '',
      libelle: '',
      note: '',
      ordre: this.formServers.length
    });
  }

  removeServer(index: number): void {
    if (index >= 0 && index < this.formServers.length) {
      this.formServers.splice(index, 1);
      //   Recalcule l'ordre pour rester cohérent
      this.formServers.forEach((s, i) => s.ordre = i);
    }
  }

  //   Vrai si la chaîne ressemble à une URL ouvrable (http/https, ftp, ou domaine www.)
  isUrl(value: string | null | undefined): boolean {
    if (!value) return false;
    const v = value.trim();
    if (!v) return false;
    return /^(https?:\/\/|ftp:\/\/|www\.)\S+$/i.test(v);
  }

  //   Ouvre une URL dans un nouvel onglet (sécurisé : noopener + noreferrer).
  //   Préfixe automatiquement avec https:// si l'utilisateur a tapé "www.…"
  openUrl(value: string | null | undefined): void {
    if (!this.isUrl(value)) return;
    let url = value!.trim();
    if (/^www\./i.test(url)) url = 'https://' + url;
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  //   Classe CSS du badge selon le type (utilisée par template + résumé)
  getServerTypeClass(type: string | undefined | null): string {
    switch ((type || '').toUpperCase()) {
      case 'DEV': return 'srv-badge srv-dev';
      case 'RECETTE': return 'srv-badge srv-recette';
      case 'PROD': return 'srv-badge srv-prod';
      case 'INT': return 'srv-badge srv-int';
      default: return 'srv-badge srv-autre';
    }
  }

  onDateChange(): void {
    if (this.formDateDebut && this.formDateFin)
      this.dateErreur = new Date(this.formDateFin) <= new Date(this.formDateDebut) ? 'La date de fin doit être après la date de début' : '';
  }

  selectCouleur(c:string): void { this.formCouleur=c; this.formCouleurCustom=''; }
  onCustomColor(): void { if(/^#[0-9a-fA-F]{6}$/.test(this.formCouleurCustom)) this.formCouleur=this.formCouleurCustom; }
  togglePrest(id:string): void { const i=this.formPrestataires.indexOf(id); i>=0?this.formPrestataires.splice(i,1):this.formPrestataires.push(id); }
  toggleResp(id:string): void { const i=this.formResponsables.indexOf(id); i>=0?this.formResponsables.splice(i,1):this.formResponsables.push(id); }
  isPrestSel(id:string): boolean { return this.formPrestataires.includes(id); }
  isRespSel(id:string): boolean { return this.formResponsables.includes(id); }

  enregistrer(): void {
    if (!this.formNom.trim()||!this.formDateDebut||!this.formDateFin) {
      this.notifService.show('error','Champs obligatoires manquants'); return;
    }
    if (this.dateErreur) { this.notifService.show('error',this.dateErreur); return; }

    //   Filtre côté front : on n'envoie que les serveurs qui ont au moins un identifiant
    //   (le backend re-filtre, mais on évite des round-trips inutiles)
    const servers = this.formServers
      .map((s, idx) => ({
        type: s.type,
        identifiant: (s.identifiant ?? '').trim(),
        libelle: (s.libelle ?? '').trim() || null,
        note: (s.note ?? '').trim() || null,
        ordre: idx
      }))
      .filter(s => !!s.identifiant);

    const dto = {
      nom: this.formNom.trim(), description: this.formDescription||undefined,
      dateDebut: new Date(this.formDateDebut).toISOString(),
      dateFin: new Date(this.formDateFin).toISOString(),
      statut: this.formStatut, priorite: this.formPriorite,
      couleur: this.formCouleur, avancement: this.formAvancement,
      prestataires: [
        ...this.formPrestataires.map(id=>({prestataireId:id,role:'DEV'})),
        ...this.formResponsables.map(id=>({prestataireId:id,role:'RESP'}))
      ],
      servers
    };

    if (this.modalMode==='create') {
      this.planningService.create(dto).subscribe({
        next:()=>{ this.notifService.show('success','Application créée'); this.closeModal(); this.loadApplications(); },
        error:()=>this.notifService.show('error','Erreur création')
      });
    } else {
      this.planningService.update(this.editingApp!.id!,dto).subscribe({
        next:()=>{ this.notifService.show('success','Application modifiée'); this.closeModal(); this.loadApplications(); },
        error:()=>this.notifService.show('error','Erreur modification')
      });
    }
  }

  confirmDelete(): void { this.appToDelete=this.editingApp; this.showModal=false; this.showDeleteConfirm=true; }
  cancelDelete(): void { this.showDeleteConfirm=false; this.appToDelete=null; }

  doDelete(): void {
    if (!this.appToDelete?.id) return;
    this.planningService.delete(this.appToDelete.id).subscribe({
      next:()=>{ this.notifService.show('success','Supprimé'); this.showDeleteConfirm=false; this.appToDelete=null; this.loadApplications(); },
      error:()=>this.notifService.show('error','Erreur suppression')
    });
  }

  // ── Drag & drop : réordonner les lignes ────────────────────────────────────
  onDragStart(event: DragEvent, index: number): void {
    this.dragFromIndex = index;
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
      event.dataTransfer.setData('text/plain', String(index));
    }
  }

  onDragOver(event: DragEvent, index: number): void {
    event.preventDefault();
    if (event.dataTransfer) event.dataTransfer.dropEffect = 'move';
    this.dragOverIndex = index;
  }

  onDrop(event: DragEvent, toIndex: number): void {
    event.preventDefault();
    const fromIndex = this.dragFromIndex;
    if (fromIndex === null || fromIndex === toIndex) {
      this.dragFromIndex = null;
      this.dragOverIndex = null;
      return;
    }
    const list = [...this.orderedApps];
    const [moved] = list.splice(fromIndex, 1);
    list.splice(toIndex, 0, moved);
    this.orderedApps = list;
    this.dragFromIndex = null;
    this.dragOverIndex = null;
    this.cdr.detectChanges();
    this.saveOrder(list);
  }

  saveOrder(list: ApplicationPlanning[]): void {
    const updates = list.map((app, index) => {
      if (!app.id) return;
      const dto: any = {
        nom: app.nom,
        description: app.description,
        dateDebut: app.dateDebut,
        dateFin: app.dateFin,
        statut: app.statut,
        priorite: app.priorite,
        couleur: app.couleur,
        avancement: app.avancement,
        ordre: index,
        prestataires: app.prestataires.map(p => ({ prestataireId: p.prestataireId, role: p.role }))
      };
      return this.planningService.update(app.id, dto).subscribe({ error: () => {} });
    });
    this.notifService.show('success', 'Ordre sauvegardé');
  }

  onDragEnd(): void { this.dragFromIndex = null; this.dragOverIndex = null; }
  onDragLeave(): void {}
  isDragOver(index: number): boolean { return this.dragOverIndex === index; }

  // ── Export Excel ───────────────────────────────────────────────────────────
  exportExcel(): void {
    const date = new Date().toLocaleDateString('fr-FR');
    const refYear = this.referenceDate.getFullYear();
    const apps = this.filteredApps;
    const years = [refYear - 2, refYear - 1, refYear, refYear + 1, refYear + 2];
    const MOIS = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jul', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc'];
    const statutBg: Record<string, string> = { 'Planifié': '#DBEAFE', 'En cours': '#FEF9C3', 'Terminé': '#DCFCE7', 'En pause': '#F3F4F6' };
    const statutFg: Record<string, string> = { 'Planifié': '#1D4ED8', 'En cours': '#A16207', 'Terminé': '#15803D', 'En pause': '#6B7280' };

    const cellState = (app: ApplicationPlanning, year: number, month: number): 'none' | 'full' => {
      const mStart = new Date(year, month, 1).getTime();
      const mEnd = new Date(year, month + 1, 0, 23, 59, 59).getTime();
      const aStart = new Date(app.dateDebut).getTime();
      const aEnd = new Date(app.dateFin).getTime();
      return aEnd < mStart || aStart > mEnd ? 'none' : 'full';
    };

    const yearHeaders = years.map(y =>
      `<td colspan="12" style="background:#1E293B;color:white;font-size:13px;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px">${y}</td>`
    ).join('');

    const monthHeaders = years.map(y => MOIS.map((m, mi) => {
      const isNow = new Date().getFullYear() === y && new Date().getMonth() === mi;
      return `<td style="width:42px;background:${isNow ? '#2563EB' : '#1E293B'};color:${isNow ? 'white' : '#94A3B8'};font-size:9px;font-weight:bold;text-align:center;border:1px solid #374151;padding:5px 2px;white-space:nowrap">${m}</td>`;
    }).join('')).join('');

    const rows = apps.map((app, i) => {
      const rowBg = i % 2 === 0 ? '#FFFFFF' : '#F8FAFC';
      const cells = years.map(y => MOIS.map((_, mi) => {
        const state = cellState(app, y, mi);
        const isNow = new Date().getFullYear() === y && new Date().getMonth() === mi;
        if (state === 'none') return `<td style="background:${isNow ? 'rgba(219,234,254,0.3)' : rowBg};border:1px solid #F1F5F9;width:42px;padding:0"></td>`;
        const alpha = isNow ? 'FF' : 'EE';
        return `<td style="background:${app.couleur};border:1px solid rgba(255,255,255,0.3);width:42px;padding:0"></td>`;
      }).join('')).join('');

      return `<tr style="height:36px">
        <td style="background:${rowBg};border:1px solid #E2E8F0;padding:6px 12px;white-space:nowrap;min-width:150px">
          <span style="display:inline-block;width:10px;height:10px;border-radius:50%;background:${app.couleur};margin-right:7px;vertical-align:middle"></span>
          <b style="color:#1E293B;font-size:12px">${app.nom}</b>
        </td>
        <td style="background:${statutBg[app.statut] || '#F3F4F6'};border:1px solid #E2E8F0;text-align:center;padding:4px 8px;white-space:nowrap;min-width:80px">
          <b style="color:${statutFg[app.statut] || '#374151'};font-size:10px">${app.statut}</b>
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;white-space:nowrap;min-width:80px;font-size:11px;color:#374151">
          ${new Date(app.dateDebut).toLocaleDateString('fr-FR')}
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;white-space:nowrap;min-width:80px;font-size:11px;color:#374151">
          ${new Date(app.dateFin).toLocaleDateString('fr-FR')}
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;font-size:11px;font-weight:bold;color:#374151">
          ${app.avancement}%
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;padding:5px 10px;font-size:11px;color:#475569">
          ${app.prestataires.filter(p => p.role !== 'RESP').map(p => p.nomComplet).join(', ') || '—'}
        </td>
        <td style="background:#EFF6FF;border:1px solid #E2E8F0;padding:5px 10px;font-size:11px;color:#2563EB;font-weight:600">
          ${app.prestataires.filter(p => p.role === 'RESP').map(p => p.nomComplet).join(', ') || '—'}
        </td>
        ${cells}
      </tr>`;
    }).join('');

    const html = `<html xmlns:o="urn:schemas-microsoft-com:office:office"
      xmlns:x="urn:schemas-microsoft-com:office:excel"
      xmlns="http://www.w3.org/TR/REC-html40">
<head><meta charset="utf-8">
<style>
  *{font-family:Calibri,Arial,sans-serif}
  table{border-collapse:collapse}
</style>
</head><body>
<table>
  <tr>
    <td colspan="${7 + years.length * 12}" style="background:#1E293B;color:white;font-size:15px;font-weight:bold;padding:12px 18px">
      📅 Planning Applications ${refYear - 2} – ${refYear + 2} &nbsp;&nbsp;
      <span style="font-size:11px;color:#94A3B8;font-weight:normal">Exporté le ${date} · ${apps.length} application(s)</span>
    </td>
  </tr>
  <tr>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:150px">APPLICATION</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:80px">STATUT</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:80px">DÉBUT</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:80px">FIN</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:60px">AVA.</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:160px">PRESTATAIRES</td>
    <td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px;min-width:160px">RESPONSABLES</td>
    ${yearHeaders}
  </tr>
  <tr>
    <td colspan="7" style="background:#0F172A;border:1px solid #374151"></td>
    ${monthHeaders}
  </tr>
  ${rows}
</table>
</body></html>`;

    const blob = new Blob(['﻿' + html], { type: 'application/vnd.ms-excel;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'Planning_' + (refYear - 2) + '-' + (refYear + 2) + '_' + date.replace(/\//g, '-') + '.xls';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    this.notifService.show('success', '✅ Planning Gantt Excel téléchargé !');
  }

  // ── Export PDF (fenêtre d'impression, 3 pages A3 paysage) ─────────────────
  exportPDF(): void {
    const date = new Date().toLocaleDateString('fr-FR');
    const refYear = this.referenceDate.getFullYear();
    const apps = this.filteredApps;
    const MOIS = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jul', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc'];
    const COL_W = 52;
    const ROW_H = 32;
    const LABEL_W = 160;
    const allYears = [refYear - 1, refYear, refYear + 1, refYear + 2, refYear + 3];
    const totalCols = allYears.length * 12;
    const tlStart = new Date(refYear - 1, 0, 1).getTime();
    const tlSpan = new Date(refYear + 4, 0, 1).getTime() - tlStart;

    const buildSvg = (yearsSlice: number[]): string => {
      const nCols = yearsSlice.length * 12;
      const svgW = LABEL_W + nCols * COL_W;
      const HEADER_H = 46;
      const svgH = HEADER_H + apps.length * ROW_H + 8;
      const sliceStart = new Date(yearsSlice[0], 0, 1).getTime();
      const sliceEnd = new Date(yearsSlice[yearsSlice.length - 1] + 1, 0, 1).getTime();
      const sliceSpan = sliceEnd - sliceStart;

      const xOf = (dateStr: string): number => {
        const t = new Date(dateStr).getTime();
        return LABEL_W + Math.max(0, Math.min(1, (t - sliceStart) / sliceSpan)) * (nCols * COL_W);
      };
      const wOf = (d1: string, d2: string): number => {
        const s = Math.max(new Date(d1).getTime(), sliceStart);
        const e = Math.min(new Date(d2).getTime(), sliceEnd);
        if (e <= s) return 0;
        return Math.max(2, ((e - s) / sliceSpan) * (nCols * COL_W));
      };

      let s = `<svg xmlns="http://www.w3.org/2000/svg" width="${svgW}" height="${svgH}" font-family="Arial,sans-serif">`;
      s += `<rect width="${svgW}" height="${svgH}" fill="#fff"/>`;

      // En-tête années
      let x = LABEL_W;
      for (const y of yearsSlice) {
        const w = 12 * COL_W;
        s += `<rect x="${x}" y="0" width="${w}" height="22" fill="#1e293b"/>`;
        s += `<text x="${x + w / 2}" y="15" fill="white" font-size="12" font-weight="bold" text-anchor="middle">${y}</text>`;
        x += w;
      }

      // En-tête mois + grille verticale
      x = LABEL_W;
      for (let c = 0; c < nCols; c++) {
        const mi = c % 12;
        const bg = mi % 2 === 0 ? '#f8fafc' : '#f1f5f9';
        s += `<rect x="${x}" y="22" width="${COL_W}" height="24" fill="${bg}"/>`;
        s += `<text x="${x + COL_W / 2}" y="36" fill="#64748b" font-size="9" text-anchor="middle">${MOIS[mi]}</text>`;
        s += `<line x1="${x}" y1="22" x2="${x}" y2="${svgH}" stroke="#e2e8f0" stroke-width="0.5"/>`;
        x += COL_W;
      }

      // Colonne libellés
      s += `<rect x="0" y="0" width="${LABEL_W}" height="${HEADER_H}" fill="#1e293b"/>`;
      s += '<text x="12" y="30" fill="white" font-size="11" font-weight="bold">APPLICATION</text>';
      s += `<line x1="${LABEL_W}" y1="0" x2="${LABEL_W}" y2="${svgH}" stroke="#e2e8f0" stroke-width="1"/>`;

      // Ligne "aujourd'hui"
      const todayX = LABEL_W + ((Date.now() - sliceStart) / sliceSpan) * (nCols * COL_W);
      if (todayX > LABEL_W && todayX < svgW) {
        s += `<line x1="${todayX}" y1="${HEADER_H}" x2="${todayX}" y2="${svgH}" stroke="#3b82f6" stroke-width="1.5" stroke-dasharray="4,2"/>`;
      }

      // Lignes applications
      for (let i = 0; i < apps.length; i++) {
        const a = apps[i];
        const y = HEADER_H + i * ROW_H;
        const rowBg = i % 2 === 0 ? '#fff' : '#f8fafc';
        s += `<rect x="0" y="${y}" width="${svgW}" height="${ROW_H}" fill="${rowBg}"/>`;
        s += `<line x1="0" y1="${y + ROW_H}" x2="${svgW}" y2="${y + ROW_H}" stroke="#e2e8f0" stroke-width="0.5"/>`;
        s += `<circle cx="12" cy="${y + ROW_H / 2}" r="4" fill="${a.couleur}"/>`;
        const nom = a.nom.length > 18 ? a.nom.slice(0, 17) + '…' : a.nom;
        s += `<text x="22" y="${y + ROW_H / 2 + 4}" fill="#1e293b" font-size="10" font-weight="600">${nom}</text>`;

        const barX = xOf(a.dateDebut);
        const barW = wOf(a.dateDebut, a.dateFin);
        if (barW > 0) {
          const by2 = y + 6;
          const bh = ROW_H - 12;
          s += `<rect x="${barX}" y="${by2}" width="${barW}" height="${bh}" rx="4" fill="${a.couleur}" opacity="0.85"/>`;
          if (a.avancement > 0) s += `<rect x="${barX}" y="${by2}" width="${barW * a.avancement / 100}" height="${bh}" rx="4" fill="${a.couleur}" opacity="0.5"/>`;
          if (barW > 30) s += `<text x="${barX + 6}" y="${by2 + bh / 2 + 4}" fill="white" font-size="9" font-weight="bold">${a.nom.slice(0, 12)}</text>`;
          if (barW > 60 && a.avancement > 0) s += `<text x="${barX + barW - 4}" y="${by2 + bh / 2 + 4}" fill="white" font-size="9" text-anchor="end">${a.avancement}%</text>`;
        }
      }

      s += '</svg>';
      return s;
    };

    const svgPage1 = buildSvg([refYear - 2, refYear - 1]);
    const svgPage2 = buildSvg([refYear, refYear + 1]);
    const svgPage3 = buildSvg([refYear + 2]);

    const tableRows = apps.map((a, i) => {
      const debut = new Date(a.dateDebut).toLocaleDateString('fr-FR');
      const fin = new Date(a.dateFin).toLocaleDateString('fr-FR');
      const duree = Math.ceil((new Date(a.dateFin).getTime() - new Date(a.dateDebut).getTime()) / 86400000);
      const prest = a.prestataires.filter((p: any) => p.role !== 'RESP').map((p: any) => p.nomComplet).join(', ') || '-';
      const resp = a.prestataires.filter((p: any) => p.role === 'RESP').map((p: any) => p.nomComplet).join(', ') || '-';
      const stBg = ({ 'Planifié': '#dbeafe', 'En cours': '#fef9c3', 'Terminé': '#dcfce7', 'En pause': '#f3f4f6' } as Record<string, string>)[a.statut] || '#f3f4f6';
      const stFg = ({ 'Planifié': '#1d4ed8', 'En cours': '#a16207', 'Terminé': '#15803d', 'En pause': '#6b7280' } as Record<string, string>)[a.statut] || '#6b7280';
      return `<tr style="background:${i % 2 === 0 ? '#fff' : '#f8fafc'}">
        <td style="padding:7px 10px"><span style="display:inline-block;width:10px;height:10px;border-radius:50%;background:${a.couleur};margin-right:7px;vertical-align:middle"></span><b>${a.nom}</b></td>
        <td style="padding:7px 10px"><span style="background:${stBg};color:${stFg};padding:2px 8px;border-radius:20px;font-size:10px;font-weight:600">${a.statut}</span></td>
        <td style="padding:7px 10px">${a.priorite}</td>
        <td style="padding:7px 10px">${debut}</td>
        <td style="padding:7px 10px">${fin}</td>
        <td style="padding:7px 10px">${duree}j</td>
        <td style="padding:7px 10px"><div style="background:#e2e8f0;border-radius:20px;height:8px;width:80px;display:inline-block;vertical-align:middle;margin-right:5px"><div style="background:${a.couleur};border-radius:20px;height:8px;width:${a.avancement * 0.8}px"></div></div>${a.avancement}%</td>
        <td style="padding:7px 10px;font-size:11px;color:#64748b">${prest}</td>
        <td style="padding:7px 10px;font-size:11px;color:#2563eb">${resp}</td>
      </tr>`;
    }).join('');

    const html = `<!DOCTYPE html>
<html><head><meta charset="utf-8"><title>Planning ${refYear - 2}–${refYear + 2}</title>
<style>
  @page { margin:1cm; size:A3 landscape; }
  * { box-sizing:border-box; }
  body { font-family:Arial,sans-serif; font-size:12px; color:#1e293b; margin:0; padding:16px; }
  .page { page-break-after: always; }
  .page:last-child { page-break-after: auto; }
  .cover { display:flex; align-items:center; justify-content:space-between; margin-bottom:16px; background:#1e293b; border-radius:10px; padding:14px 20px; }
  .cover-title { font-size:20px; font-weight:800; color:white; }
  .cover-sub { font-size:12px; color:#94a3b8; margin-top:3px; }
  .cover-badge { background:#2563eb; color:white; padding:6px 16px; border-radius:8px; font-size:13px; font-weight:700; }
  .section-title { font-size:11px; font-weight:700; color:#64748b; text-transform:uppercase; letter-spacing:.5px; margin:12px 0 6px; }
  .gantt-wrap { overflow:hidden; border-radius:8px; border:1px solid #e2e8f0; }
  .year-label { background:#1e293b; color:white; font-size:13px; font-weight:700; padding:6px 14px; border-radius:6px; display:inline-block; margin-bottom:8px; }
  table { width:100%; border-collapse:collapse; }
  thead tr { background:#1e293b; }
  thead th { padding:8px 10px; text-align:left; font-size:10px; font-weight:700; color:white; text-transform:uppercase; }
  tbody tr:nth-child(even) { background:#f8fafc; }
  td { border-bottom:1px solid #f1f5f9; }
  .footer { margin-top:12px; font-size:10px; color:#94a3b8; text-align:center; border-top:1px solid #e2e8f0; padding-top:8px; }
</style></head>
<body>

<!-- PAGE 1 : Couverture + Gantt 2025-2026 -->
<div class="page">
  <div class="cover">
    <div>
      <div class="cover-title">📅 Planning Applications ${refYear - 2} – ${refYear + 2}</div>
      <div class="cover-sub">${apps.length} application(s) · Exporté le ${date}</div>
    </div>
    <div class="cover-badge">KRONOS</div>
  </div>
  <div class="section-title">Vue Gantt — <span class="year-label">${refYear - 2} – ${refYear - 1}</span></div>
  <div class="gantt-wrap">${svgPage1}</div>
  <div class="footer">Page 1/3 · KRONOS · ${date}</div>
</div>

<!-- PAGE 2 : Gantt 2027-2028 -->
<div class="page">
  <div class="cover">
    <div><div class="cover-title">📅 Planning — suite</div><div class="cover-sub">${apps.length} application(s) · ${date}</div></div>
    <div class="cover-badge">KRONOS</div>
  </div>
  <div class="section-title">Vue Gantt — <span class="year-label">${refYear} – ${refYear + 1}</span></div>
  <div class="gantt-wrap">${svgPage2}</div>
  <div class="footer">Page 2/3 · KRONOS · ${date}</div>
</div>

<!-- PAGE 3 : Gantt 2029 + Tableau -->
<div class="page">
  <div class="section-title">Vue Gantt — <span class="year-label">${refYear + 2}</span></div>
  <div class="gantt-wrap">${svgPage3}</div>
  <div class="section-title" style="margin-top:16px">Détail des applications</div>
  <table>
    <thead><tr>
      <th>Application</th><th>Statut</th><th>Priorité</th>
      <th>Début</th><th>Fin</th><th>Durée</th><th>Avancement</th>
      <th>Prestataires</th><th>Responsables</th>
    </tr></thead>
    <tbody>${tableRows}</tbody>
  </table>
  <div class="footer">Page 3/3 · KRONOS · ${date}</div>
</div>

</body></html>`;

    const win = window.open('', '_blank', 'width=1400,height=900');
    if (win) {
      win.document.write(html);
      win.document.close();
      setTimeout(() => { win.print(); }, 800);
    }
  }

  // ── Tooltip ────────────────────────────────────────────────────────────────
  showTip(event: MouseEvent, app: ApplicationPlanning): void {
    this.tooltip = { show: true, app, x: event.clientX + 14, y: event.clientY - 8 };
  }
  hideTip(): void { this.tooltip = { ...this.tooltip, show: false }; }

  // ── Helpers d'affichage ────────────────────────────────────────────────────
  getStatutClass(s: string): string {
    return ({ 'Planifié': 'st-p', 'En cours': 'st-e', 'Terminé': 'st-t', 'En pause': 'st-x' } as Record<string, string>)[s] ?? 'st-p';
  }

  getInitials(nom:string,prenom:string): string { return ((nom?.[0]??'')+(prenom?.[0]??'')).toUpperCase(); }

  getAvatarUrl(p: Prestataire | null | undefined): string | null {
    this.imageUrl = '';
    this.imageUrl = p?.imageUrl!;
    if (!p?.imageUrl) return null;
    return p?.imageUrl.startsWith('http') ? this.imageUrl : `${environment.apiUrl.replace('/api', '')}/${this.imageUrl}`;
  }

  getAvatarUrlById(id: string): string | null {
    const all = [...this.prestataires, ...this.responsables];
    const p = all.find(x => x.id === id);
    return p ? this.getAvatarUrl(p) : null;
  }

  getInitialsFromNom(nomComplet: string): string {
    const parts = nomComplet.trim().split(' ');
    if (parts.length >= 2) return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    return (parts[0]?.[0] ?? '?').toUpperCase();
  }

  getPrestInitialsById(id:string): string { const p=this.prestataires.find(x=>x.id===id); return p?this.getInitials(p.nom,p.prenom):'?'; }
  getPrestNomById(id:string): string { const p=this.prestataires.find(x=>x.id===id); return p?`${p.nom} ${p.prenom}`:''; }
  getRespInitialsById(id:string): string { const r=this.responsables.find(x=>x.id===id); return r?this.getInitials(r.nom,r.prenom):'?'; }
  getRespNomById(id:string): string { const r=this.responsables.find(x=>x.id===id); return r?`${r.nom} ${r.prenom}`:''; }
  getFilterLabel(): string { if(!this.filterPrestId) return 'Prestataires'; const p=[...this.prestataires,...this.responsables].find(x=>x.id===this.filterPrestId); return p?`${p.nom} ${p.prenom}`:'Prestataires'; }

  getAppPrestNames(app: ApplicationPlanning): string {
    return app.prestataires.slice(0,3).map(p=>p.nomComplet).join(', ')+(app.prestataires.length>3?` +${app.prestataires.length-3}`:'');
  }

  // ── Helpers de dates ───────────────────────────────────────────────────────
  private getMonday(d: Date): Date {
    const day = d.getDay();
    const diff = d.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(d.getFullYear(), d.getMonth(), diff);
  }

  private sameDay(a: Date, b: Date): boolean {
    return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
  }

  private sameWeek(a: Date, b: Date): boolean {
    return this.sameDay(this.getMonday(a), this.getMonday(b));
  }

  private isoWeek(d: Date): number {
    const date = new Date(d);
    date.setHours(0, 0, 0, 0);
    date.setDate(date.getDate() + 3 - ((date.getDay() + 6) % 7));
    const week1 = new Date(date.getFullYear(), 0, 4);
    return 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + ((week1.getDay() + 6) % 7)) / 7);
  }

  //   Ferme les menus déroulants quand on clique en dehors
  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    const t = e.target as HTMLElement;
    if (!t.closest('.pf-wrap')) this.showPrestFilter = false;
    if (!t.closest('#ddp')) this.showDdPrest = false;
    if (!t.closest('#ddr')) this.showDdResp = false;
  }
}
