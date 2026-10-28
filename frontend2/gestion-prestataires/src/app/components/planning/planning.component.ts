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

  @ViewChild('ganttScroll') ganttScrollRef!: ElementRef<HTMLElement>;

  // ── Data ──────────────────────────────────────────────────────────────────
  applications: ApplicationPlanning[] = [];
  prestataires: Prestataire[] = [];
  responsables: Prestataire[] = [];
  //   Toggle "Inclure désactivés" : filtres + dropdowns du modal
  showInactivePrest = false;
  isLoading = false;

  // ── View ──────────────────────────────────────────────────────────────────
  viewMode: ViewMode = 'year';
  referenceDate = new Date();
  readonly ROW_HEIGHT = ROW_HEIGHT;

  imageUrl? : string = '' ;

  
  // ── Timeline (computed) ───────────────────────────────────────────────────
  units: TimelineUnit[] = [];
  groups: TimelineGroup[] = [];
  cellWidth = CELL_W['year'];
  totalWidth = 0;

  // ── Filtre prestataire ─────────────────────────────────────────────────────
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

  // ── Modal delete ──────────────────────────────────────────────────────────
  showDeleteConfirm = false;
  appToDelete: ApplicationPlanning | null = null;

  // ── Tooltip ───────────────────────────────────────────────────────────────
  tooltip: { show: boolean; app: ApplicationPlanning | null; x: number; y: number } =
    { show: false, app: null, x: 0, y: 0 };

  // ── Drag & Drop sidebar ───────────────────────────────────────────────────
  dragFromIndex: number | null = null;
  dragOverIndex: number | null = null;
  orderedApps: ApplicationPlanning[] = [];

  readonly COULEURS_PRESET = [
    '#3b82f6','#6366f1','#8b5cf6','#a855f7','#ec4899',
    '#ef4444','#f97316','#f59e0b','#eab308','#84cc16',
    '#22c55e','#10b981','#14b8a6','#06b6d4'
  ];
  readonly STATUTS  = ['Planifié','En cours','Terminé','En pause'];
  readonly PRIORITES = ['Faible','Moyenne','Haute','Critique'];

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

  // ── Load ──────────────────────────────────────────────────────────────────
  loadApplications(): void {
    this.isLoading = true;
    this.planningService.getAll().subscribe({
      next: d => {
          this.applications = d;
          // Trier par ordre si le champ existe en base
          this.orderedApps = [...d].sort((a, b) => {
            const ao = (a as any).ordre ?? 9999;
            const bo = (b as any).ordre ?? 9999;
            return ao - bo;
          });
          this.isLoading = false;
          this.cdr.detectChanges();
        },
      error: () => { this.notifService.show('error','Erreur chargement'); this.isLoading = false; }
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

  // ── Timeline ──────────────────────────────────────────────────────────────
  buildTimeline(): void {
    this.cellWidth = CELL_W[this.viewMode];
    this.units = [];
    this.groups = [];
    const today = new Date();

    if (this.viewMode === 'year') {
      // Unités = MOIS sur 5 ans : année-2 → année+2
      const startYear = this.referenceDate.getFullYear() - 1;
      const endYear   = startYear + 4;
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
      // Mode MOIS = toute l'année courante en semaines (S1→S52/53)
      // Groupes = mois (jan, fév, mar, ...)
      const y0 = this.referenceDate.getFullYear();
      // Première semaine ISO de l'année
      let d = this.getMonday(new Date(y0, 0, 4));
      // Dernier jour de l'année
      const lastDay = new Date(y0, 11, 31);
      const groupMap = new Map<string, number>();
      while (d.getFullYear() <= y0 && d <= lastDay) {
        const wn  = this.isoWeek(d);
        // Groupe = mois du jeudi de la semaine (norme ISO)
        const thu = new Date(d); thu.setDate(d.getDate() + 3);
        const key = thu.toLocaleDateString('fr-FR', { month: 'short' });
        groupMap.set(key, (groupMap.get(key) || 0) + 1);
        this.units.push({ date: new Date(d), label: `S${wn}`, isToday: this.sameWeek(today, d) });
        d = new Date(d); d.setDate(d.getDate() + 7);
        if (d.getFullYear() > y0) break;
      }
      this.groups = Array.from(groupMap.entries()).map(([label, unitCount]) => ({ label, unitCount }));

    } else {
      // Mode Semaine : jours du lundi de la date de référence
      // jusqu'à la fin de la dernière application (ou 6 semaines min)
      const monday = this.getMonday(this.referenceDate);

      // Trouver la date de fin max parmi toutes les applications
      let maxEnd = new Date(monday);
      maxEnd.setDate(monday.getDate() + 41); // 6 semaines minimum
      for (const app of this.applications) {
        const appEnd = new Date(app.dateFin);
        if (appEnd > maxEnd) maxEnd = appEnd;
      }
      // Trouver la date de début min
      let minStart = new Date(monday);
      for (const app of this.applications) {
        const appStart = new Date(app.dateDebut);
        if (appStart < minStart) minStart = appStart;
      }
      // Commencer au lundi de la date min
      const startMonday = this.getMonday(minStart);

      const groupMap = new Map<string, number>();
      let d = new Date(startMonday);
      while (d <= maxEnd) {
        const key = d.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' });
        groupMap.set(key, (groupMap.get(key) || 0) + 1);
        this.units.push({ date: new Date(d), label: d.toLocaleDateString('fr-FR', { weekday: 'short', day: 'numeric' }), isToday: this.sameDay(today, d) });
        d = new Date(d); d.setDate(d.getDate() + 1);
      }
      this.groups = Array.from(groupMap.entries()).map(([label, unitCount]) => ({ label, unitCount }));
    }

    this.totalWidth = this.units.length * this.cellWidth;
    setTimeout(() => this.scrollToToday(), 50);
  }

  private scrollToToday(): void {
    if (!this.ganttScrollRef) return;
    const pos = this.getTodayPx();
    if (pos !== null) {
      this.ganttScrollRef.nativeElement.scrollLeft = Math.max(0, pos - 300);
    }
  }

  // ── Navigation ────────────────────────────────────────────────────────────
  previous(): void {
    if (this.viewMode==='year') this.referenceDate = new Date(this.referenceDate.getFullYear()-1, 6, 1);
    else if (this.viewMode==='month') this.referenceDate = new Date(this.referenceDate.getFullYear()-1, 0, 1);
    else { const d = new Date(this.referenceDate); d.setDate(d.getDate()-42); this.referenceDate=d; }
    this.buildTimeline();
  }

  next(): void {
    if (this.viewMode==='year') this.referenceDate = new Date(this.referenceDate.getFullYear()+1, 6, 1);
    else if (this.viewMode==='month') this.referenceDate = new Date(this.referenceDate.getFullYear()+1, 0, 1);
    else { const d = new Date(this.referenceDate); d.setDate(d.getDate()+42); this.referenceDate=d; }
    this.buildTimeline();
  }

  goToday(): void { this.referenceDate = new Date(); this.buildTimeline(); }
  setView(m: ViewMode): void { this.viewMode = m; this.buildTimeline(); }

  get periodLabel(): string {
    if (this.viewMode==='year') return `${this.referenceDate.getFullYear()-2} – ${this.referenceDate.getFullYear()+2}`;
    if (this.viewMode==='month') return `${this.referenceDate.getFullYear()}`;
    return this.referenceDate.toLocaleDateString('fr-FR',{month:'long',year:'numeric'});
  }

  // ── Bar position (en px, comme React) ────────────────────────────────────
  getBarPosition(app: ApplicationPlanning): BarPosition | null {
    if (!this.units.length) return null;
    const start = new Date(app.dateDebut).getTime();
    const end   = new Date(app.dateFin).getTime();
    const unitMs = this.viewMode === 'week'
      ? 86400000                    // 1 jour
      : this.viewMode === 'year'
        ? 30.4375 * 86400000        // ~1 mois
        : 7 * 86400000;             // 1 semaine

    const tStart = this.units[0].date.getTime();
    const tEnd   = this.units[this.units.length-1].date.getTime() + unitMs;

    if (end <= tStart || start >= tEnd) return null;

    const clampedStart = Math.max(start, tStart);
    const clampedEnd   = Math.min(end, tEnd);
    const totalMs = this.units.length * unitMs;

    const left  = ((clampedStart - tStart) / totalMs) * this.totalWidth;
    const width = Math.max(((clampedEnd - clampedStart) / totalMs) * this.totalWidth, 4);

    return { left: Math.round(left), width: Math.round(width) };
  }

  getTodayPx(): number | null {
    if (!this.units.length) return null;
    const now = Date.now();
    const unitMs = this.viewMode === 'week'
      ? 86400000                    // 1 jour
      : this.viewMode === 'year'
        ? 30.4375 * 86400000        // ~1 mois
        : 7 * 86400000;             // 1 semaine
    const tStart = this.units[0].date.getTime();
    const totalMs = this.units.length * unitMs;
    const px = ((now - tStart) / totalMs) * this.totalWidth;
    if (px < 0 || px > this.totalWidth) return null;
    return Math.round(px);
  }

  get todayLineStyle(): { [k:string]: string } {
    const px = this.getTodayPx();
    if (px === null) return { display:'none' };
    return { left: `${px}px` };
  }

  get totalHeightPx(): number { return this.filteredApps.length * ROW_HEIGHT; }
  getRowTop(i: number): number { return i * ROW_HEIGHT; }

  darkenColor(hex: string, amt: number): string {
    const c = hex.replace('#','');
    if (c.length!==6) return hex;
    const n = parseInt(c,16);
    const r = Math.max(0,(n>>16)-amt), g = Math.max(0,((n>>8)&255)-amt), b = Math.max(0,(n&255)-amt);
    return `#${((r<<16)|(g<<8)|b).toString(16).padStart(6,'0')}`;
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

  // ── Drag & Drop ──────────────────────────────────────────────────────────
  onDragStart(e: DragEvent, index: number): void {
    this.dragFromIndex = index;
    if (e.dataTransfer) {
      e.dataTransfer.effectAllowed = 'move';
      e.dataTransfer.setData('text/plain', String(index));
    }
  }

  onDragOver(e: DragEvent, index: number): void {
    e.preventDefault();
    if (e.dataTransfer) e.dataTransfer.dropEffect = 'move';
    this.dragOverIndex = index;
  }

  onDrop(e: DragEvent, toIndex: number): void {
    e.preventDefault();
    const fromIndex = this.dragFromIndex;
    if (fromIndex === null || fromIndex === toIndex) {
      this.dragFromIndex = null;
      this.dragOverIndex = null;
      return;
    }
    // Réordonner localement
    const items = [...this.orderedApps];
    const [moved] = items.splice(fromIndex, 1);
    items.splice(toIndex, 0, moved);
    this.orderedApps = items;
    this.dragFromIndex = null;
    this.dragOverIndex = null;
    this.cdr.detectChanges();

    // Persister l'ordre en base : envoyer un PATCH ordre pour chaque app
    this.saveOrder(items);
  }

  private saveOrder(items: ApplicationPlanning[]): void {
    // Envoyer le nouvel ordre via update pour chaque application
    // On utilise l'index comme valeur d'ordre
    const updates = items.map((app, idx) => {
      if (!app.id) return;
      const dto = {
        nom: app.nom,
        description: app.description,
        dateDebut: app.dateDebut,
        dateFin: app.dateFin,
        statut: app.statut,
        priorite: app.priorite,
        couleur: app.couleur,
        avancement: app.avancement,
        ordre: idx,
        prestataires: app.prestataires.map(p => ({ prestataireId: p.prestataireId, role: p.role })),
        //   On renvoie les serveurs existants : sinon l'API les remplaçait par une liste vide
        //   (serveurs supprimés à chaque réordonnancement)
        ...(app.servers ? {
          servers: app.servers.map((s, i) => ({
            type: s.type,
            identifiant: s.identifiant,
            libelle: s.libelle ?? null,
            note: s.note ?? null,
            ordre: s.ordre ?? i
          }))
        } : {})
      };
      return this.planningService.update(app.id, dto as any).subscribe({
        error: () => {} // silencieux — l'ordre local est déjà appliqué
      });
    });
    // Notif discrète
    this.notifService.show('success', 'Ordre sauvegardé');
  }

  onDragEnd(): void {
    this.dragFromIndex = null;
    this.dragOverIndex = null;
  }

  onDragLeave(): void {
    // reset visuel si on quitte la zone
  }

  isDragOver(i: number): boolean { return this.dragOverIndex === i; }

  // ── Export Excel Gantt (cellules colorées) ────────────────────────────────
  exportExcel(): void {
    const today  = new Date().toLocaleDateString('fr-FR');
    const year   = this.referenceDate.getFullYear();
    const apps   = this.filteredApps;
    const YEARS  = [year-2, year-1, year, year+1, year+2];
    const MONTHS = ['Jan','Fév','Mar','Avr','Mai','Jun','Jul','Aoû','Sep','Oct','Nov','Déc'];

    const stBg: Record<string,string> = {'Planifié':'#DBEAFE','En cours':'#FEF9C3','Terminé':'#DCFCE7','En pause':'#F3F4F6'};
    const stCl: Record<string,string> = {'Planifié':'#1D4ED8','En cours':'#A16207','Terminé':'#15803D','En pause':'#6B7280'};

    // Pour chaque app × chaque mois : est-ce que la barre passe dans ce mois ?
    const getMonthState = (app: any, y: number, m: number): 'full'|'start'|'end'|'none' => {
      const mStart = new Date(y, m, 1).getTime();
      const mEnd   = new Date(y, m + 1, 0, 23, 59, 59).getTime();
      const aStart = new Date(app.dateDebut).getTime();
      const aEnd   = new Date(app.dateFin).getTime();
      if (aEnd < mStart || aStart > mEnd) return 'none';
      return 'full';
    };

    // Construire header années
    const yearCols = YEARS.map(y =>
      `<td colspan="12" style="background:#1E293B;color:white;font-size:13px;font-weight:bold;text-align:center;border:1px solid #374151;padding:7px">${y}</td>`
    ).join('');

    // Header mois
    const monthCols = YEARS.map(y =>
      MONTHS.map((m, mi) => {
        const isNow = new Date().getFullYear()===y && new Date().getMonth()===mi;
        return `<td style="width:42px;background:${isNow?'#2563EB':'#1E293B'};color:${isNow?'white':'#94A3B8'};font-size:9px;font-weight:bold;text-align:center;border:1px solid #374151;padding:5px 2px;white-space:nowrap">${m}</td>`;
      }).join('')
    ).join('');

    // Lignes applications
    const appRows = apps.map((a: any, i: number) => {
      const rowBg = i%2===0 ? '#FFFFFF' : '#F8FAFC';

      const ganttCells = YEARS.map(y =>
        MONTHS.map((m, mi) => {
          const state = getMonthState(a, y, mi);
          const isNow = new Date().getFullYear()===y && new Date().getMonth()===mi;
          if (state === 'none') {
            return `<td style="background:${isNow?'rgba(219,234,254,0.3)':rowBg};border:1px solid #F1F5F9;width:42px;padding:0"></td>`;
          }
          // Cellule active : couleur de l'app
          const alpha = isNow ? 'FF' : 'EE';
          return `<td style="background:${a.couleur};border:1px solid rgba(255,255,255,0.3);width:42px;padding:0"></td>`;
        }).join('')
      ).join('');

      return `<tr style="height:36px">
        <td style="background:${rowBg};border:1px solid #E2E8F0;padding:6px 12px;white-space:nowrap;min-width:150px">
          <span style="display:inline-block;width:10px;height:10px;border-radius:50%;background:${a.couleur};margin-right:7px;vertical-align:middle"></span>
          <b style="color:#1E293B;font-size:12px">${a.nom}</b>
        </td>
        <td style="background:${stBg[a.statut]||'#F3F4F6'};border:1px solid #E2E8F0;text-align:center;padding:4px 8px;white-space:nowrap;min-width:80px">
          <b style="color:${stCl[a.statut]||'#374151'};font-size:10px">${a.statut}</b>
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;white-space:nowrap;min-width:80px;font-size:11px;color:#374151">
          ${new Date(a.dateDebut).toLocaleDateString('fr-FR')}
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;white-space:nowrap;min-width:80px;font-size:11px;color:#374151">
          ${new Date(a.dateFin).toLocaleDateString('fr-FR')}
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;text-align:center;padding:4px;font-size:11px;font-weight:bold;color:#374151">
          ${a.avancement}%
        </td>
        <td style="background:${rowBg};border:1px solid #E2E8F0;padding:5px 10px;font-size:11px;color:#475569">
          ${a.prestataires.filter((p:any)=>p.role!=='RESP').map((p:any)=>p.nomComplet).join(', ')||'—'}
        </td>
        <td style="background:#EFF6FF;border:1px solid #E2E8F0;padding:5px 10px;font-size:11px;color:#2563EB;font-weight:600">
          ${a.prestataires.filter((p:any)=>p.role==='RESP').map((p:any)=>p.nomComplet).join(', ')||'—'}
        </td>
        ${ganttCells}
      </tr>`;
    }).join('');

    const xls = `<html xmlns:o="urn:schemas-microsoft-com:office:office"
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
    <td colspan="${7 + YEARS.length*12}" style="background:#1E293B;color:white;font-size:15px;font-weight:bold;padding:12px 18px">
      📅 Planning Applications ${year-2} – ${year+2} &nbsp;&nbsp;
      <span style="font-size:11px;color:#94A3B8;font-weight:normal">Exporté le ${today} · ${apps.length} application(s)</span>
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
    ${yearCols}
  </tr>
  <tr>
    <td colspan="7" style="background:#0F172A;border:1px solid #374151"></td>
    ${monthCols}
  </tr>
  ${appRows}
</table>
</body></html>`;

    const blob = new Blob(['﻿' + xls], { type: 'application/vnd.ms-excel;charset=utf-8' });
    const url  = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href     = url;
    link.download = 'Planning_' + (year-2) + '-' + (year+2) + '_' + today.replace(/\//g,'-') + '.xls';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
    this.notifService.show('success', '✅ Planning Gantt Excel téléchargé !');
  }

  // ── Export PDF ────────────────────────────────────────────────────────────
  exportPDF(): void {
    const today   = new Date().toLocaleDateString('fr-FR');
    const year    = this.referenceDate.getFullYear();
    const apps    = this.filteredApps;

    const MONTHS  = ['Jan','Fév','Mar','Avr','Mai','Jun','Jul','Aoû','Sep','Oct','Nov','Déc'];
    const COL_W   = 52;
    const ROW_H   = 32;
    const SB_W    = 160;
    // 5 ans : année-2 → année+2
    const YEARS   = [year-1, year, year+1, year+2, year+3];
    const TOTAL_MONTHS = YEARS.length * 12;
    const timeStart = new Date(year - 1, 0, 1).getTime();
    const timeEnd   = new Date(year + 4, 0, 1).getTime();
    const totalMs   = timeEnd - timeStart;

    // ── Construire le Gantt SVG (une page par paire d'années) ─────────────
    // On génère 3 SVGs : [2025-2026] [2027-2028] [2029]
    const buildSvg = (yearsSlice: number[]) => {
      const NM   = yearsSlice.length * 12;
      const W    = SB_W + NM * COL_W;
      const HDR  = 46;
      const H    = HDR + apps.length * ROW_H + 8;
      const tS   = new Date(yearsSlice[0], 0, 1).getTime();
      const tE   = new Date(yearsSlice[yearsSlice.length-1] + 1, 0, 1).getTime();
      const totMs = tE - tS;

      const bx = (d: string) => {
        const t = new Date(d).getTime();
        return SB_W + Math.max(0, Math.min(1, (t - tS) / totMs)) * (NM * COL_W);
      };
      const bw = (s: string, e: string) => {
        const cs = Math.max(new Date(s).getTime(), tS);
        const ce = Math.min(new Date(e).getTime(), tE);
        if (ce <= cs) return 0;
        return Math.max(2, ((ce - cs) / totMs) * (NM * COL_W));
      };

      let s = `<svg xmlns="http://www.w3.org/2000/svg" width="${W}" height="${H}" font-family="Arial,sans-serif">`;
      s += `<rect width="${W}" height="${H}" fill="#fff"/>`;

      // Années
      let xOff = SB_W;
      for (const y of yearsSlice) {
        const w = 12 * COL_W;
        s += `<rect x="${xOff}" y="0" width="${w}" height="22" fill="#1e293b"/>`;
        s += `<text x="${xOff+w/2}" y="15" fill="white" font-size="12" font-weight="bold" text-anchor="middle">${y}</text>`;
        xOff += w;
      }
      // Mois
      xOff = SB_W;
      for (let i = 0; i < NM; i++) {
        const m = i % 12;
        const bg = m%2===0 ? '#f8fafc' : '#f1f5f9';
        s += `<rect x="${xOff}" y="22" width="${COL_W}" height="24" fill="${bg}"/>`;
        s += `<text x="${xOff+COL_W/2}" y="36" fill="#64748b" font-size="9" text-anchor="middle">${MONTHS[m]}</text>`;
        s += `<line x1="${xOff}" y1="22" x2="${xOff}" y2="${H}" stroke="#e2e8f0" stroke-width="0.5"/>`;
        xOff += COL_W;
      }
      // Sidebar header
      s += `<rect x="0" y="0" width="${SB_W}" height="${HDR}" fill="#1e293b"/>`;
      s += `<text x="12" y="30" fill="white" font-size="11" font-weight="bold">APPLICATION</text>`;
      s += `<line x1="${SB_W}" y1="0" x2="${SB_W}" y2="${H}" stroke="#e2e8f0" stroke-width="1"/>`;
      // Ligne aujourd'hui
      const todayX = SB_W + ((Date.now() - tS) / totMs) * (NM * COL_W);
      if (todayX > SB_W && todayX < W) {
        s += `<line x1="${todayX}" y1="${HDR}" x2="${todayX}" y2="${H}" stroke="#3b82f6" stroke-width="1.5" stroke-dasharray="4,2"/>`;
      }
      // Rows
      for (let i = 0; i < apps.length; i++) {
        const a = apps[i];
        const y0 = HDR + i * ROW_H;
        const bg = i%2===0 ? '#fff' : '#f8fafc';
        s += `<rect x="0" y="${y0}" width="${W}" height="${ROW_H}" fill="${bg}"/>`;
        s += `<line x1="0" y1="${y0+ROW_H}" x2="${W}" y2="${y0+ROW_H}" stroke="#e2e8f0" stroke-width="0.5"/>`;
        s += `<circle cx="12" cy="${y0+ROW_H/2}" r="4" fill="${a.couleur}"/>`;
        const nom = a.nom.length > 18 ? a.nom.slice(0,17)+'…' : a.nom;
        s += `<text x="22" y="${y0+ROW_H/2+4}" fill="#1e293b" font-size="10" font-weight="600">${nom}</text>`;
        const barX = bx(a.dateDebut);
        const barW = bw(a.dateDebut, a.dateFin);
        if (barW > 0) {
          const by2 = y0+6, bh = ROW_H-12;
          s += `<rect x="${barX}" y="${by2}" width="${barW}" height="${bh}" rx="4" fill="${a.couleur}" opacity="0.85"/>`;
          if (a.avancement>0) s += `<rect x="${barX}" y="${by2}" width="${barW*a.avancement/100}" height="${bh}" rx="4" fill="${a.couleur}" opacity="0.5"/>`;
          if (barW>30) s += `<text x="${barX+6}" y="${by2+bh/2+4}" fill="white" font-size="9" font-weight="bold">${a.nom.slice(0,12)}</text>`;
          if (barW>60 && a.avancement>0) s += `<text x="${barX+barW-4}" y="${by2+bh/2+4}" fill="white" font-size="9" text-anchor="end">${a.avancement}%</text>`;
        }
      }
      s += '</svg>';
      return s;
    };

    // 3 pages Gantt : 2 ans / 2 ans / 1 an
    const svg1 = buildSvg([year-2, year-1]);
    const svg2 = buildSvg([year, year+1]);
    const svg3 = buildSvg([year+2]);

    // ── Tableau récap ─────────────────────────────────────────────────────
    const rows = apps.map((a: any, idx: number) => {
      const d    = new Date(a.dateDebut).toLocaleDateString('fr-FR');
      const f    = new Date(a.dateFin).toLocaleDateString('fr-FR');
      const dur  = Math.ceil((new Date(a.dateFin).getTime()-new Date(a.dateDebut).getTime())/86400000);
      const prest= a.prestataires.filter((p:any)=>p.role!=='RESP').map((p:any)=>p.nomComplet).join(', ')||'-';
      const resp = a.prestataires.filter((p:any)=>p.role==='RESP').map((p:any)=>p.nomComplet).join(', ')||'-';
      const stBg = ({'Planifié':'#dbeafe','En cours':'#fef9c3','Terminé':'#dcfce7','En pause':'#f3f4f6'} as any)[a.statut]||'#f3f4f6';
      const stCl = ({'Planifié':'#1d4ed8','En cours':'#a16207','Terminé':'#15803d','En pause':'#6b7280'} as any)[a.statut]||'#6b7280';
      const bg   = idx%2===0?'#fff':'#f8fafc';
      return `<tr style="background:${bg}">
        <td style="padding:7px 10px"><span style="display:inline-block;width:10px;height:10px;border-radius:50%;background:${a.couleur};margin-right:7px;vertical-align:middle"></span><b>${a.nom}</b></td>
        <td style="padding:7px 10px"><span style="background:${stBg};color:${stCl};padding:2px 8px;border-radius:20px;font-size:10px;font-weight:600">${a.statut}</span></td>
        <td style="padding:7px 10px">${a.priorite}</td>
        <td style="padding:7px 10px">${d}</td>
        <td style="padding:7px 10px">${f}</td>
        <td style="padding:7px 10px">${dur}j</td>
        <td style="padding:7px 10px"><div style="background:#e2e8f0;border-radius:20px;height:8px;width:80px;display:inline-block;vertical-align:middle;margin-right:5px"><div style="background:${a.couleur};border-radius:20px;height:8px;width:${a.avancement*0.8}px"></div></div>${a.avancement}%</td>
        <td style="padding:7px 10px;font-size:11px;color:#64748b">${prest}</td>
        <td style="padding:7px 10px;font-size:11px;color:#2563eb">${resp}</td>
      </tr>`;
    }).join('');

    const html = `<!DOCTYPE html>
<html><head><meta charset="utf-8"><title>Planning ${year-2}–${year+2}</title>
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
      <div class="cover-title">📅 Planning Applications ${year-2} – ${year+2}</div>
      <div class="cover-sub">${apps.length} application(s) · Exporté le ${today}</div>
    </div>
    <div class="cover-badge">KRONOS</div>
  </div>
  <div class="section-title">Vue Gantt — <span class="year-label">${year-2} – ${year-1}</span></div>
  <div class="gantt-wrap">${svg1}</div>
  <div class="footer">Page 1/3 · KRONOS · ${today}</div>
</div>

<!-- PAGE 2 : Gantt 2027-2028 -->
<div class="page">
  <div class="cover">
    <div><div class="cover-title">📅 Planning — suite</div><div class="cover-sub">${apps.length} application(s) · ${today}</div></div>
    <div class="cover-badge">KRONOS</div>
  </div>
  <div class="section-title">Vue Gantt — <span class="year-label">${year} – ${year+1}</span></div>
  <div class="gantt-wrap">${svg2}</div>
  <div class="footer">Page 2/3 · KRONOS · ${today}</div>
</div>

<!-- PAGE 3 : Gantt 2029 + Tableau -->
<div class="page">
  <div class="section-title">Vue Gantt — <span class="year-label">${year+2}</span></div>
  <div class="gantt-wrap">${svg3}</div>
  <div class="section-title" style="margin-top:16px">Détail des applications</div>
  <table>
    <thead><tr>
      <th>Application</th><th>Statut</th><th>Priorité</th>
      <th>Début</th><th>Fin</th><th>Durée</th><th>Avancement</th>
      <th>Prestataires</th><th>Responsables</th>
    </tr></thead>
    <tbody>${rows}</tbody>
  </table>
  <div class="footer">Page 3/3 · KRONOS · ${today}</div>
</div>

</body></html>`;

    const w = window.open('','_blank','width=1400,height=900');
    if (!w) return;
    w.document.write(html);
    w.document.close();
    setTimeout(()=>{ w.print(); }, 800);
  }

  // ── Tooltip ───────────────────────────────────────────────────────────────
  showTip(e:MouseEvent,app:ApplicationPlanning): void { this.tooltip={show:true,app,x:e.clientX+14,y:e.clientY-8}; }
  hideTip(): void { this.tooltip={...this.tooltip,show:false}; }

  // ── Helpers ───────────────────────────────────────────────────────────────
  getStatutClass(s:string): string {
    return {'Planifié':'st-p','En cours':'st-e','Terminé':'st-t','En pause':'st-x'}[s]??'st-p';
  }

  getInitials(nom:string,prenom:string): string { return ((nom?.[0]??'')+(prenom?.[0]??'')).toUpperCase(); }
  getAvatarUrl(p:Prestataire): string|null {
    this.imageUrl ='';
        this.imageUrl =p?.imageUrl;
    if (!p?.imageUrl) return null;
    if (p?.imageUrl.startsWith('http')) return this.imageUrl!;
        return `${environment.apiUrl.replace('/api', '')}/${this.imageUrl}`;
    }

  // Helpers sidebar avatars
  getAvatarUrlById(id:string): string|null {
    const all = [...this.prestataires,...this.responsables];
    const p = all.find(x=>x.id===id);
    return p ? this.getAvatarUrl(p) : null;
  }

  getInitialsFromNom(nomComplet:string): string {
    const parts = nomComplet.trim().split(' ');
    if (parts.length >= 2) return (parts[0][0]+(parts[parts.length-1][0])).toUpperCase();
    return (parts[0]?.[0]??'?').toUpperCase();
  }

  getPrestInitialsById(id:string): string { const p=this.prestataires.find(x=>x.id===id); return p?this.getInitials(p.nom,p.prenom):'?'; }
  getPrestNomById(id:string): string { const p=this.prestataires.find(x=>x.id===id); return p?`${p.nom} ${p.prenom}`:''; }
  getRespInitialsById(id:string): string { const r=this.responsables.find(x=>x.id===id); return r?this.getInitials(r.nom,r.prenom):'?'; }
  getRespNomById(id:string): string { const r=this.responsables.find(x=>x.id===id); return r?`${r.nom} ${r.prenom}`:''; }
  getFilterLabel(): string { if(!this.filterPrestId) return 'Prestataires'; const p=[...this.prestataires,...this.responsables].find(x=>x.id===this.filterPrestId); return p?`${p.nom} ${p.prenom}`:'Prestataires'; }

  // App prestataires pour sidebar
  getAppPrestNames(app:ApplicationPlanning): string {
    return app.prestataires.slice(0,3).map(p=>p.nomComplet).join(', ')+(app.prestataires.length>3?` +${app.prestataires.length-3}`:'');
  }

  private getMonday(d:Date): Date { const day=d.getDay(),diff=d.getDate()-day+(day===0?-6:1); return new Date(d.getFullYear(),d.getMonth(),diff); }
  private sameDay(a:Date,b:Date): boolean { return a.getFullYear()===b.getFullYear()&&a.getMonth()===b.getMonth()&&a.getDate()===b.getDate(); }
  private sameWeek(a:Date,b:Date): boolean { return this.sameDay(this.getMonday(a),this.getMonday(b)); }
  private isoWeek(d:Date): number { const date=new Date(d); date.setHours(0,0,0,0); date.setDate(date.getDate()+3-((date.getDay()+6)%7)); const w1=new Date(date.getFullYear(),0,4); return 1+Math.round(((date.getTime()-w1.getTime())/86400000-3+((w1.getDay()+6)%7))/7); }

  @HostListener('document:click',['$event'])
  onDocClick(e:MouseEvent): void {
    const t = e.target as HTMLElement;
    if (!t.closest('.pf-wrap')) this.showPrestFilter=false;
    if (!t.closest('#ddp')) this.showDdPrest=false;
    if (!t.closest('#ddr')) this.showDdResp=false;
  }
}