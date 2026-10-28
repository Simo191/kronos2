import { ChangeDetectorRef, Component, HostListener, OnInit, OnDestroy } from '@angular/core';
import { ShiftService } from '../../services/shift.service';
import { Shift, TypeShift, ShiftCategory } from '../../models/Shift.model';
import { Prestataire } from '../../models/prestataire.model';
import { PrestataireService } from '../../services/prestataire.service';

export interface Notification {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  autoClose?: boolean;
}
import { JourTravailService, JourTravailDto } from '../../services/jour-travail.service';
import { ContratService, ContratDto } from '../../services/contrat.service';
import { MoisVerrouilleService } from '../../services/verrouille.service';   // ✅ NOUVEAU
import { CommonModule, registerLocaleData } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import localeFr from '@angular/common/locales/fr';
import { PermissionService, Permission, Role } from '../../services/permission.service';
import { environment } from '../../../environments/environment';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';

registerLocaleData(localeFr);

// Interface pour le résultat de génération
interface GenerationResult {
  prestataireNom: string;
  prestatairePrenom: string;
  mois: string;
  nombreJours: number;
  tjm: number;
  montantTotal: number;
  status: 'success' | 'quota_depasse' | 'contrat_introuvable' | 'error' | 'Interne';
  message: string;
  quotaContrat?: number;
  joursUtilises?: number;
}

@Component({
  selector: 'app-shift-planning',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationToastComponent],
  templateUrl: './shift-planning.component.html',
  styleUrls: ['./shift-planning.component.scss']
})
export class ShiftPlanningComponent implements OnInit, OnDestroy {
  // Vue
  viewMode: 'month' | 'week' = 'week';
  currentDate: Date = new Date();

  // Données
  shifts: Shift[] = [];
  prestataires: Prestataire[] = [];
  typeShifts: TypeShift[] = [];
  contrats: ContratDto[] = [];                // ✅ NOUVEAU : cache contrats pour indicateurs

  // Filtres
  selectedPrestataireId: string | null = null;
  filterStartDate: string = '';
  filterEndDate: string = '';

  // Modal
  showModal = false;
  modalMode: 'create' | 'edit' = 'create';
  selectedShift: Shift | null = null;
  selectedDate: Date | null = null;
  selectedPrestataire: string = '';
  selectedTypeShiftId: string = '';
  shiftNotes = '';
  nombreJours: number = 1;

  // Intervalle de dates
  isDateRangeEnabled = false;
  rangeStartDate: string = '';
  rangeEndDate: string = '';

  // Menu contextuel
  contextMenu = {
    show: false,
    x: 0,
    y: 0,
    type: '' as 'shift' | 'cell',
    targetDay: null as Date | null,
    targetPrestataire: null as Prestataire | null
  };
  contextMenuShift: Shift | null = null;

  // Copier/Coller
  copiedData: {
    shifts: Shift[];
    dates: Date[];
    prestataire: string;
  } | null = null;

  selectedShifts: Set<string> = new Set();

  // Rubber-band
  isDragging = false;
  dragStart: { x: number; y: number } | null = null;
  dragRect: { left: number; top: number; width: number; height: number } | null = null;
  private dragContainer: HTMLElement | null = null;

  // Génération automatique jours-travail
  showGenerationModal = false;
  generationInProgress = false;
  generationResults: GenerationResult[] = [];
  selectedMonthForGeneration: string = '';
  generationComplete = false;   // ✅ NOUVEAU : indique si la génération est terminée

  // ✅ NOUVEAU : verrouillage post-génération
  lockingMonth = false;

  // Modal de confirmation
  showConfirmModal = false;
  confirmTitle = '';

  // Modal d'erreur
  showErrorModal = false;

  // Notifications
  errorMessage = '';
  confirmMessage = '';
  confirmAction: (() => void) | null = null;

  constructor(
    private shiftService: ShiftService,
    private prestataireService: PrestataireService,
    private jourTravailService: JourTravailService,
    private contratService: ContratService,
    private moisVerrouilleService: MoisVerrouilleService,  // ✅ NOUVEAU
    private cdr: ChangeDetectorRef,
    private permissionService: PermissionService,
    private notifService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadPrestataires();
    this.loadTypeShifts();
    this.loadContrats();   // ✅ NOUVEAU
    this.setupKeyboardListeners();

    this.selectedMonthForGeneration = this.formatMonthForInput(this.currentDate);

    document.addEventListener('click', () => this.closeContextMenu());
  }

  ngOnDestroy(): void {
    document.removeEventListener('click', () => this.closeContextMenu());
  }

  // ========== PERMISSIONS ==========

  canManage(): boolean {
    return this.permissionService.hasPermission(Permission.MANAGE_PLANNING);
  }

  canModifyShift(shift: Shift): boolean {
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    if (isAdmin || isManager) return true;
    const userPrestataireId = this.getCurrentUserPrestataireId();
    if (!userPrestataireId) return false;
    return shift.prestataireId === userPrestataireId;
  }

  canAddForPrestataire(prestataireId: string | null): boolean {
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    if (isAdmin || isManager) return true;
    const userPrestataireId = this.getCurrentUserPrestataireId();
    if (!userPrestataireId || !prestataireId) return false;
    return prestataireId === userPrestataireId;
  }

  canAddOnDay(): boolean {
    if (!this.contextMenu.targetDay || !this.contextMenu.targetPrestataire) return false;
    return this.canAddForCell(this.contextMenu.targetPrestataire, this.contextMenu.targetDay);
  }

  canAddForCell(prestataire: Prestataire, day?: Date): boolean {
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);

    if (day && !isAdmin) {
      if (this.isWeekend(day) || this.isJourFerie(day)) return false;
    }
    if (isAdmin) return true;
    if (isManager) return true;

    const userPrestataireId = this.getCurrentUserPrestataireId();
    if (!userPrestataireId) return false;
    return prestataire.id === userPrestataireId;
  }

  getContextMenuPrestataireId(): string | null {
    if (!this.contextMenu.targetPrestataire) return null;
    if (typeof this.contextMenu.targetPrestataire === 'string') return this.contextMenu.targetPrestataire;
    return this.contextMenu.targetPrestataire.id || null;
  }

  private getCurrentUserPrestataireId(): string | null {
    // ✅ SÉCURITÉ : lire depuis le JWT (signé par le serveur), jamais depuis currentUser localStorage
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64).split('').map((c: string) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
      );
      const payload = JSON.parse(json);
      if (payload.exp && payload.exp * 1000 < Date.now()) return null;
      return payload['prestataireId'] || null;
    } catch (error) {
      console.error('❌ Erreur lecture token JWT:', error);
      return null;
    }
  }

  // ========== CHARGEMENT DES DONNÉES ==========

  loadPrestataires(): void {
    this.prestataireService.getPrestataireShift().subscribe({
      next: (data) => {
        // ✅ Trier par champ `order` si disponible
        this.prestataires = data.sort((a, b) => (a.order ?? 999) - (b.order ?? 999));
        this.loadShifts();
      },
      error: (err) => console.error('Erreur chargement prestataires:', err)
    });
  }

  loadTypeShifts(): void {
    this.shiftService.getAllTypeShifts().subscribe({
      next: (data) => { this.typeShifts = data; },
      error: (err) => console.error('Erreur chargement types:', err)
    });
  }

  // ✅ NOUVEAU : Charger les contrats pour les indicateurs visuels
  loadContrats(): void {
    this.contratService.getAll().subscribe({
      next: (data) => {
        this.contrats = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Erreur chargement contrats:', err)
    });
  }

  loadShifts(): void {
    let startDate: Date;
    let endDate: Date;

    if (this.viewMode === 'week') {
      const startOfWeek = this.getStartOfWeek(this.currentDate);
      startDate = new Date(startOfWeek);
      startDate.setHours(12, 0, 0, 0);
      endDate = new Date(startOfWeek);
      endDate.setDate(endDate.getDate() + 6);
      endDate.setHours(12, 0, 0, 0);
    } else {
      const year = this.currentDate.getFullYear();
      const month = this.currentDate.getMonth();
      startDate = new Date(year, month, 1, 12, 0, 0);
      endDate = new Date(year, month + 1, 0, 12, 0, 0);
    }

    this.shiftService.getShiftsByDateRange(startDate, endDate, this.selectedPrestataireId).subscribe({
      next: (shifts) => {
        this.shifts = shifts;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('❌ Erreur chargement shifts:', err)
    });
  }

  // ========== ✅ DRAG & DROP ORDRE PRESTATAIRES ==========
  draggedPrestataire: Prestataire | null = null;
  dragOverPrestataireId: string | null = null;

  onDragStart(event: DragEvent, prestataire: Prestataire): void {
    this.draggedPrestataire = prestataire;
    event.dataTransfer!.effectAllowed = 'move';
    // Needed for Firefox
    event.dataTransfer!.setData('text/plain', prestataire.id || '');
  }

  onDragOver(event: DragEvent, prestataire: Prestataire): void {
    event.preventDefault();
    event.dataTransfer!.dropEffect = 'move';
    this.dragOverPrestataireId = prestataire.id || null;
  }

  onDragLeave(): void {
    this.dragOverPrestataireId = null;
  }

  onDrop(event: DragEvent, targetPrestataire: Prestataire): void {
    event.preventDefault();
    this.dragOverPrestataireId = null;
    if (!this.draggedPrestataire || this.draggedPrestataire.id === targetPrestataire.id) {
      this.draggedPrestataire = null;
      return;
    }
    const fromIndex = this.prestataires.findIndex(p => p.id === this.draggedPrestataire!.id);
    const toIndex = this.prestataires.findIndex(p => p.id === targetPrestataire.id);
    if (fromIndex < 0 || toIndex < 0) { this.draggedPrestataire = null; return; }

    // Réorganiser le tableau localement
    const updated = [...this.prestataires];
    const [removed] = updated.splice(fromIndex, 1);
    updated.splice(toIndex, 0, removed);

    // Réassigner les ordres 0, 1, 2, ...
    updated.forEach((p, idx) => { p.order = idx; });
    this.prestataires = updated;
    this.cdr.detectChanges();

    // Persister en base
    updated.forEach(p => this.updatePrestataireOrder(p.id!, p.order!));
    this.draggedPrestataire = null;
  }

  onDragEnd(): void {
    this.draggedPrestataire = null;
    this.dragOverPrestataireId = null;
  }

  /**
   * Déplacer un prestataire vers le haut dans la liste
   */
  movePrestataireUp(prestataire: Prestataire): void {
    const index = this.prestataires.findIndex(p => p.id === prestataire.id);
    if (index <= 0) return;

    const prev = this.prestataires[index - 1];
    const prevOrder = prev.order ?? index - 1;
    const currOrder = prestataire.order ?? index;

    // Swap orders
    this.updatePrestataireOrder(prestataire.id!, prevOrder);
    this.updatePrestataireOrder(prev.id!, currOrder);

    // Update local state immediately for UX
    this.prestataires[index - 1] = { ...prev, order: currOrder };
    this.prestataires[index] = { ...prestataire, order: prevOrder };
    this.prestataires = [...this.prestataires];
    this.cdr.detectChanges();
  }

  /**
   * Déplacer un prestataire vers le bas dans la liste
   */
  movePrestataireDown(prestataire: Prestataire): void {
    const index = this.prestataires.findIndex(p => p.id === prestataire.id);
    if (index < 0 || index >= this.prestataires.length - 1) return;

    const next = this.prestataires[index + 1];
    const nextOrder = next.order ?? index + 1;
    const currOrder = prestataire.order ?? index;

    this.updatePrestataireOrder(prestataire.id!, nextOrder);
    this.updatePrestataireOrder(next.id!, currOrder);

    // Update local state
    this.prestataires[index + 1] = { ...next, order: currOrder };
    this.prestataires[index] = { ...prestataire, order: nextOrder };
    this.prestataires = [...this.prestataires];
    this.cdr.detectChanges();
  }

  private updatePrestataireOrder(prestataireId: string, order: number): void {
    const p = this.prestataires.find(x => x.id === prestataireId);
    if (!p) return;
    this.prestataireService.update(prestataireId, { ...p, order }).subscribe({
      error: (err) => console.error('Erreur update order prestataire:', err)
    });
  }

  // ========== ✅ NOUVEAU : INDICATEUR CONTRAT PAR PRESTATAIRE ==========

  /**
   * Récupérer le contrat actif d'un prestataire à la date courante
   */
  getContratActifForPrestataire(prestataireId: string): ContratDto | null {
    const today = new Date();
    return this.contrats.find(c => {
      if (c.prestataireId !== prestataireId) return false;
      if (!(c.active ?? true)) return false;
      const debut = new Date(c.dateDebut);
      const fin = c.dateFin ? new Date(c.dateFin) : new Date(2099, 11, 31);
      return debut <= today && today <= fin;
    }) || null;
  }

  /**
   * Statut couleur du contrat pour un prestataire
   * 'danger' = dépassé ou expiré, 'warning' = proche (<=20j), 'success' = ok, null = pas de contrat
   */
  getContratStatus(prestataireId: string): 'danger' | 'warning' | 'success' | null {
    const p = this.prestataires.find(x => x.id === prestataireId);
    if (p?.responsable) return null;  // Les responsables n'ont pas de contrat

    const contrat = this.getContratActifForPrestataire(prestataireId);
    if (!contrat) return null;

    // Cas 1 : date expirée
    if (contrat.dateFin) {
      const fin = new Date(contrat.dateFin);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (fin < today) return 'danger';
    }

    // Cas 2 : jours restants
    const restants = contrat.nombreJours - (contrat.joursUtilises ?? 0);
    if (restants < 0) return 'danger';
    if (restants <= 20) return 'warning';
    return 'success';
  }

  getContratJoursRestants(prestataireId: string): number | null {
    const contrat = this.getContratActifForPrestataire(prestataireId);
    if (!contrat) return null;
    return contrat.nombreJours - (contrat.joursUtilises ?? 0);
  }

  // ========== GÉNÉRATION AUTOMATIQUE JOURS-TRAVAIL ==========

  openGenerationModal(): void {
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);

    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      if (!userPrestataireId) {
        this.showError('Impossible de déterminer votre prestataire');
        return;
      }
    }

    this.selectedMonthForGeneration = this.formatMonthForInput(this.currentDate);
    this.showGenerationModal = true;
    this.generationResults = [];
    this.generationComplete = false;
  }

  closeGenerationModal(): void {
    this.showGenerationModal = false;
    this.generationResults = [];
    this.generationComplete = false;
  }

  async generateJoursTravail(): Promise<void> {
    if (!this.selectedMonthForGeneration) {
      this.showError('Veuillez sélectionner un mois');
      return;
    }

    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    let allowedPrestataireIds: string[] = [];

    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      if (!userPrestataireId) {
        this.showError('Impossible de déterminer votre prestataire');
        return;
      }
      allowedPrestataireIds = [userPrestataireId];
    } else {
      allowedPrestataireIds = this.prestataires.map(p => p.id!);
    }

    this.generationInProgress = true;
    this.generationResults = [];
    this.generationComplete = false;
    this.cdr.detectChanges();

    try {
      const [year, month] = this.selectedMonthForGeneration.split('-').map(Number);
      const monthStart = new Date(year, month - 1, 1);
      const monthEnd = new Date(year, month, 0, 23, 59, 59);

      const shifts = await this.shiftService.getShiftsByDateRange(monthStart, monthEnd, null).toPromise();

      if (!shifts || shifts.length === 0) {
        this.showError('Aucun shift trouvé pour ce mois');
        this.generationInProgress = false;
        this.cdr.detectChanges();
        return;
      }

      const [joursTravailExistants, contrats] = await Promise.all([
        this.jourTravailService.getAll().toPromise(),
        this.contratService.getAll().toPromise()
      ]);

      const shiftsByPrestataire = this.groupShiftsByPrestataire(shifts);

      for (const [prestataireId, prestataireShifts] of Object.entries(shiftsByPrestataire)) {
        if (!allowedPrestataireIds.includes(prestataireId)) continue;

        const prestataire = this.prestataires.find(p => p.id === prestataireId);
        if (!prestataire) continue;

        // Compter les JOURS UNIQUES travaillés
        const shiftsTravailes = prestataireShifts.filter(s =>
          s.typeShift.category === ShiftCategory.Travail
        );

        const joursUniques = new Set(
          shiftsTravailes.map(s => {
            const d = new Date(s.date);
            return `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;
          })
        );
        const joursTravailes = joursUniques.size;
        if (joursTravailes === 0) continue;

        // ❌ Les responsables sont ignorés : pas de génération de jours-travail
        if (prestataire.responsable) {
          continue;
        }

        // Prestataire normal : vérifier le contrat
        const contrat = this.findActiveContrat(contrats || [], prestataireId, monthStart);

        if (!contrat) {
          this.generationResults.push({
            prestataireNom: prestataire.nom,
            prestatairePrenom: prestataire.prenom,
            mois: this.selectedMonthForGeneration,
            nombreJours: joursTravailes,
            tjm: prestataire.tjm || 0,
            montantTotal: (prestataire.tjm || 0) * joursTravailes,
            status: 'contrat_introuvable',
            message: 'Aucun contrat actif trouvé pour cette période'
          });
          continue;
        }

        const quotaContrat = contrat.nombreJours;
        // ✅ Exclure l'entrée du mois EN COURS de génération pour ce prestataire
        // → si on re-génère février, ne pas compter les 7j existants de février
        //   sinon : 7 (existants) + 8 (planning) > quota → faux quota dépassé
        const joursTravailSansCurrentMois = (joursTravailExistants || []).filter(
          jt => !(jt.prestataireId === prestataireId && jt.mois === this.selectedMonthForGeneration)
        );
        const joursUtilises = this.calculateJoursUtilises(
          joursTravailSansCurrentMois,
          prestataireId,
          contrat.dateDebut,
          monthEnd
        );
        const joursDisponibles = quotaContrat - joursUtilises;

        let joursAGenerer = joursTravailes;
        let messageSeuilInfo = '';

        if (contrat.seuilJoursMois && contrat.seuilJoursMois > 0) {
          if (joursTravailes > contrat.seuilJoursMois) {
            joursAGenerer = contrat.seuilJoursMois;
            messageSeuilInfo = ` | ⚠️ Seuil mensuel: ${joursTravailes}j demandés mais seuil=${contrat.seuilJoursMois}j → ${joursAGenerer}j générés`;
          }
        }

        if (joursAGenerer > joursDisponibles) {
          this.generationResults.push({
            prestataireNom: prestataire.nom,
            prestatairePrenom: prestataire.prenom,
            mois: this.selectedMonthForGeneration,
            nombreJours: joursTravailes,
            tjm: contrat.tjm,
            montantTotal: contrat.tjm * joursTravailes,
            status: 'quota_depasse',
            message: `Quota dépassé (${joursTravailes} jours demandés, ${joursDisponibles} disponibles)`,
            quotaContrat,
            joursUtilises
          });
          continue;
        }

        const jourTravailExistant = (joursTravailExistants || []).find(jt =>
          jt.prestataireId === prestataireId && jt.mois === this.selectedMonthForGeneration
        );

        try {
          const jourTravailData: JourTravailDto = {
            prestataireId,
            mois: this.selectedMonthForGeneration,
            nombreJours: joursAGenerer,
            tjm: contrat.tjm,
            description: `Généré automatiquement depuis le planning`
          };

          if (jourTravailExistant) {
            await this.jourTravailService.update(jourTravailExistant.id!, jourTravailData).toPromise();
          } else {
            await this.jourTravailService.create(jourTravailData).toPromise();
          }

          this.generationResults.push({
            prestataireNom: prestataire.nom,
            prestatairePrenom: prestataire.prenom,
            mois: this.selectedMonthForGeneration,
            nombreJours: joursTravailes,
            tjm: contrat.tjm,
            montantTotal: contrat.tjm * joursTravailes,
            status: 'success',
            message: (jourTravailExistant ? 'Mis à jour avec succès' : 'Créé avec succès') + messageSeuilInfo,
            quotaContrat,
            joursUtilises: joursUtilises + joursTravailes
          });
        } catch (error : any) {
          console.error('❌ Erreur:', error);
          console.error('❌ Erreur:', error.error);
          const msg = error?.error?.error || error?.error?.message || error?.message || 'Erreur inconnue';

          this.generationResults.push({
            prestataireNom: prestataire.nom,
            prestatairePrenom: prestataire.prenom,
            mois: this.selectedMonthForGeneration,
            nombreJours: joursTravailes,
            tjm: contrat.tjm,
            montantTotal: contrat.tjm * joursTravailes,
            status: 'error',
            message: `Erreur:  ${msg}`
          });
        }
      }

      this.generationInProgress = false;
      this.generationComplete = true;   // ✅ NOUVEAU : génération terminée → bouton verrouillage
      this.cdr.detectChanges();

    } catch (error) {
      console.error('💥 Erreur génération:', error);
      this.showError('Erreur lors de la génération des jours-travail: ' + error);
      this.generationInProgress = false;
      this.cdr.detectChanges();
    }
  }

  // ✅ NOUVEAU : Verrouiller le mois après génération
  verrouilerMoisApresGeneration(): void {
    if (!this.selectedMonthForGeneration) return;
    this.lockingMonth = true;
    this.moisVerrouilleService.verrouiller(
      this.selectedMonthForGeneration,
      `Verrouillé automatiquement après génération des jours-travail`
    ).subscribe({
      next: () => {
        this.lockingMonth = false;
        this.showSuccess(`✅ Mois ${this.selectedMonthForGeneration} verrouillé avec succès !`);
        this.closeGenerationModal();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.lockingMonth = false;
        const msg = err?.error?.message || 'Erreur lors du verrouillage';
        this.showError(msg);
        this.cdr.detectChanges();
      }
    });
  }

  private groupShiftsByPrestataire(shifts: Shift[]): { [prestataireId: string]: Shift[] } {
    return shifts.reduce((acc, shift) => {
      if (!acc[shift.prestataireId]) acc[shift.prestataireId] = [];
      acc[shift.prestataireId].push(shift);
      return acc;
    }, {} as { [prestataireId: string]: Shift[] });
  }

  private findActiveContrat(contrats: ContratDto[], prestataireId: string, referenceDate: Date): ContratDto | null {
    // referenceDate = premier jour du mois généré (ex: 2026-02-01)
    // Un contrat est valide s'il CHEVAUCHE le mois : dateDebut <= fin_du_mois ET dateFin >= début_du_mois
    // Exemple : contrat 18/02→08/03 chevauche février car 18/02 <= 28/02 ET 08/03 >= 01/02 ✅
    const monthStart = new Date(referenceDate.getFullYear(), referenceDate.getMonth(), 1);
    const monthEnd   = new Date(referenceDate.getFullYear(), referenceDate.getMonth() + 1, 0, 23, 59, 59);

    const candidats = contrats.filter(c => {
      if (c.prestataireId !== prestataireId) return false;
      if (!(c.active ?? true)) return false;  // Contrat inactif → ignoré

      const dateDebut = new Date(c.dateDebut);
      const dateFin   = c.dateFin ? new Date(c.dateFin) : new Date(2099, 11, 31);

      // Chevauchement : contrat couvre au moins un jour du mois
      return dateDebut <= monthEnd && dateFin >= monthStart;
    });

    if (candidats.length === 0) return null;

    // En cas de plusieurs contrats, prendre celui dont la dateDebut est la plus récente
    candidats.sort((a, b) => new Date(b.dateDebut).getTime() - new Date(a.dateDebut).getTime());
    return candidats[0];
  }

  private calculateJoursUtilises(
    joursTravail: JourTravailDto[],
    prestataireId: string,
    contratDebut: string,
    dateReference: Date
  ): number {
    const contratStart = new Date(contratDebut);
    return joursTravail
      .filter(jt => {
        if (jt.prestataireId !== prestataireId) return false;
        const [year, month] = jt.mois.split('-').map(Number);
        // Chevauchement mois/contrat : évite d'exclure février quand contratStart=18/02
        const moisDebut = new Date(year, month - 1, 1);
        const moisFin   = new Date(year, month, 0, 23, 59, 59);
        return moisDebut <= dateReference && moisFin >= contratStart;
      })
      .reduce((sum, jt) => sum + jt.nombreJours, 0);
  }

  getResultsCount(status: 'success' | 'quota_depasse' | 'contrat_introuvable' | 'error' | 'Interne'): number {
    return this.generationResults.filter(r => r.status === status).length;
  }

  getResultsByStatus(status: 'success' | 'quota_depasse' | 'contrat_introuvable' | 'error' | 'Interne'): GenerationResult[] {
    return this.generationResults.filter(r => r.status === status);
  }

  // ========== GESTION DE LA VUE ==========

  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'month' ? 'week' : 'month';
    this.loadShifts();
  }

  previousPeriod(): void {
    if (this.viewMode === 'month') {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
    } else {
      this.currentDate = new Date(this.currentDate.getTime() - 7 * 24 * 60 * 60 * 1000);
    }
    this.loadShifts();
  }

  nextPeriod(): void {
    if (this.viewMode === 'month') {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
    } else {
      this.currentDate = new Date(this.currentDate.getTime() + 7 * 24 * 60 * 60 * 1000);
    }
    this.loadShifts();
  }

  getCurrentPeriodLabel(): string {
    if (this.viewMode === 'month') {
      return this.currentDate.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' });
    } else {
      const days = this.getWeekDays();
      const start = days[0];
      const end = days[days.length - 1];
      return `${start.getDate()} - ${end.getDate()} ${end.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' })}`;
    }
  }

  // ========== CALENDRIER ==========

  getAllDaysForView(): Date[] {
    return this.viewMode === 'week' ? this.getWeekDays() : this.getMonthDays();
  }

  getWeekDays(): Date[] {
    const days: Date[] = [];
    const startOfWeek = this.getStartOfWeek(this.currentDate);
    for (let i = 0; i < 7; i++) {
      const day = new Date(startOfWeek);
      day.setDate(day.getDate() + i);
      day.setHours(12, 0, 0, 0);
      days.push(day);
    }
    return days;
  }

  getMonthDays(): Date[] {
    const days: Date[] = [];
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day, 12, 0, 0));
    }
    return days;
  }

  getStartOfWeek(date: Date): Date {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(d.setDate(diff));
  }

  getShiftsForDay(day: Date, prestataireId: string): Shift[] {
    return this.shifts.filter(shift => {
      const shiftDate = new Date(shift.date);
      return shift.prestataireId === prestataireId &&
             shiftDate.getDate() === day.getDate() &&
             shiftDate.getMonth() === day.getMonth() &&
             shiftDate.getFullYear() === day.getFullYear();
    });
  }

  getDayName(date: Date): string {
    return date.toLocaleDateString('fr-FR', { weekday: 'short' }).toUpperCase();
  }

  private getNextJourOuvrable(date: Date): Date {
    const nextDate = new Date(date);
    while (!this.isJourOuvrable(nextDate)) {
      nextDate.setDate(nextDate.getDate() + 1);
    }
    return nextDate;
  }

  isWeekend(date: Date): boolean {
    const day = date.getDay();
    return day === 0 || day === 6;
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return date.getDate() === today.getDate() &&
           date.getMonth() === today.getMonth() &&
           date.getFullYear() === today.getFullYear();
  }

  isJourFerie(date: Date): boolean {
    const year = date.getFullYear();
    const joursFeries = this.getJoursFeries(year);
    return joursFeries.some(ferie =>
      ferie.getDate() === date.getDate() &&
      ferie.getMonth() === date.getMonth() &&
      ferie.getFullYear() === date.getFullYear()
    );
  }

  private isJourOuvrable(date: Date): boolean {
    return !this.isWeekend(date) && !this.isJourFerie(date);
  }

  private getJoursFeries(year: number): Date[] {
    const joursFeries: Date[] = [];
    joursFeries.push(new Date(year, 0, 1));
    joursFeries.push(new Date(year, 4, 1));
    joursFeries.push(new Date(year, 4, 8));
    joursFeries.push(new Date(year, 6, 14));
    joursFeries.push(new Date(year, 7, 15));
    joursFeries.push(new Date(year, 10, 1));
    joursFeries.push(new Date(year, 10, 11));
    joursFeries.push(new Date(year, 11, 25));

    const paques = this.calculateEaster(year);
    joursFeries.push(new Date(paques.getTime() + 1 * 24 * 60 * 60 * 1000));
    joursFeries.push(new Date(paques.getTime() + 39 * 24 * 60 * 60 * 1000));
    joursFeries.push(new Date(paques.getTime() + 50 * 24 * 60 * 60 * 1000));
    return joursFeries;
  }

  private calculateEaster(year: number): Date {
    const a = year % 19;
    const b = Math.floor(year / 100);
    const c = year % 100;
    const d = Math.floor(b / 4);
    const e = b % 4;
    const f = Math.floor((b + 8) / 25);
    const g = Math.floor((b - f + 1) / 3);
    const h = (19 * a + b - d - g + 15) % 30;
    const i = Math.floor(c / 4);
    const k = c % 4;
    const l = (32 + 2 * e + 2 * i - h - k) % 7;
    const m = Math.floor((a + 11 * h + 22 * l) / 451);
    const month = Math.floor((h + l - 7 * m + 114) / 31) - 1;
    const day = ((h + l - 7 * m + 114) % 31) + 1;
    return new Date(year, month, day);
  }

  // ========== FILTRES ==========

  getFilteredPrestataires(): Prestataire[] {
    const pid = (this.selectedPrestataireId ?? '').trim();
    if (pid !== '' && pid !== 'null' && pid !== 'undefined') {
      return this.prestataires.filter(p => p.id === pid);
    }
    return this.prestataires;
  }

  onPrestataireFilterChange(): void {}
  applyDateRangeFilter(): void {}
  clearFilters(): void {
    this.selectedPrestataireId = null;
    this.filterStartDate = '';
    this.filterEndDate = '';
  }

  // ========== STATISTIQUES ==========

  getJoursTravail(): number {
    const responsableIds = new Set(this.prestataires.filter(p => p.responsable).map(p => p.id));
    const uniquePrestataireDates = new Set<string>();
    this.shifts
      .filter(s => s.typeShift?.category === ShiftCategory.Travail && !responsableIds.has(s.prestataireId))
      .forEach(s => {
        const dateKey = new Date(s.date).toISOString().split('T')[0];
        uniquePrestataireDates.add(`${s.prestataireId}|${dateKey}`);
      });
    return uniquePrestataireDates.size;
  }

  getJoursAbsence(): number {
    const responsableIds = new Set(this.prestataires.filter(p => p.responsable).map(p => p.id));
    return this.shifts.filter(s => s.typeShift.category === ShiftCategory.Absence && !responsableIds.has(s.prestataireId)).length;
  }

  getJoursConge(): number {
    const responsableIds = new Set(this.prestataires.filter(p => p.responsable).map(p => p.id));
    return this.shifts.filter(s => s.typeShift.category === ShiftCategory.Conge && !responsableIds.has(s.prestataireId)).length;
  }

  // ✅ FIX : Statistiques filtrées pour le rôle User (uniquement ses propres shifts)
  getJoursTravailUser(): number {
    const pid = this.getCurrentUserPrestataireId();
    if (!pid) return 0;
    const uniqueDates = new Set<string>();
    this.shifts
      .filter(s => s.prestataireId === pid && s.typeShift?.category === ShiftCategory.Travail)
      .forEach(s => uniqueDates.add(new Date(s.date).toISOString().split('T')[0]));
    return uniqueDates.size;
  }

  getJoursAbsenceUser(): number {
    const pid = this.getCurrentUserPrestataireId();
    if (!pid) return 0;
    return this.shifts.filter(s => s.prestataireId === pid && s.typeShift.category === ShiftCategory.Absence).length;
  }

  getJoursCongeUser(): number {
    const pid = this.getCurrentUserPrestataireId();
    if (!pid) return 0;
    return this.shifts.filter(s => s.prestataireId === pid && s.typeShift.category === ShiftCategory.Conge).length;
  }

  getJoursTravailForPrestataire(prestataireId: string): number {
    const uniqueDates = new Set<string>();
    this.shifts
      .filter(s => s.prestataireId === prestataireId && s.typeShift?.category === ShiftCategory.Travail)
      .forEach(s => {
        const dateKey = new Date(s.date).toISOString().split('T')[0];
        uniqueDates.add(dateKey);
      });
    return uniqueDates.size;
  }

  // ========== INTERACTIONS SHIFT/CELLULE ==========

  onShiftClick(event: MouseEvent, shift: Shift): void {
    event.stopPropagation();
    if (event.ctrlKey || event.metaKey) {
      if (this.selectedShifts.has(shift.id)) {
        this.selectedShifts.delete(shift.id);
      } else {
        this.selectedShifts.add(shift.id);
      }
    } else {
      if (!this.canModifyShift(shift)) {
        this.showError('Vous ne pouvez modifier que vos propres shifts');
        return;
      }
      this.openEditModal(shift);
    }
  }

  onShiftRightClick(event: MouseEvent, shift: Shift, day: Date, prestataire: Prestataire): void {
    event.preventDefault();
    event.stopPropagation();
    this.contextMenuShift = shift;
    if (!this.selectedShifts.has(shift.id)) {
      this.selectedShifts.clear();
      this.selectedShifts.add(shift.id);
    }
    this.contextMenu = { show: true, x: event.clientX, y: event.clientY, type: 'shift', targetDay: day, targetPrestataire: prestataire };
  }

  onCellClick(event: MouseEvent, day: Date, prestataire: Prestataire): void {
    const target = event.target as HTMLElement;
    if (target.closest('.shift-badge')) return;

    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);

    if (!isAdmin && (this.isWeekend(day) || this.isJourFerie(day))) {
      this.showError('Impossible d\'ajouter un shift les jours fériés ou week-ends. Seul l\'administrateur peut le faire.');
      return;
    }

    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      if (!userPrestataireId) { this.showError('Impossible de déterminer votre prestataire'); return; }
      if (prestataire.id !== userPrestataireId) { this.showError('Vous ne pouvez ajouter que vos propres shifts'); return; }
    }

    this.openCreateModal(day, prestataire);
  }

  onCellRightClick(event: MouseEvent, day: Date, prestataire: Prestataire): void {
    const shifts = this.getShiftsForDay(day, prestataire.id!);
    if (shifts.length > 0) return;
    event.preventDefault();
    event.stopPropagation();
    this.contextMenu = { show: true, x: event.clientX, y: event.clientY, type: 'cell', targetDay: day, targetPrestataire: prestataire };
  }

  isShiftSelected(shiftId: string): boolean {
    return this.selectedShifts.has(shiftId);
  }

  // ========== MENU CONTEXTUEL ==========

  closeContextMenu(): void { this.contextMenu.show = false; }

  editShiftFromContext(): void {
    const selectedShiftObjects = this.shifts.filter(s => this.selectedShifts.has(s.id));
    if (selectedShiftObjects.length === 1 && !this.canModifyShift(selectedShiftObjects[0])) {
      this.showError('Vous ne pouvez modifier que vos propres shifts');
      this.closeContextMenu();
      return;
    }
    if (selectedShiftObjects.length === 1) this.openEditModal(selectedShiftObjects[0]);
    this.closeContextMenu();
  }

  copyShiftFromContext(): void { this.copySelectedShifts(); this.closeContextMenu(); }

  addShiftFromContext(): void {
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);

    if (!isAdmin && this.contextMenu.targetDay && (this.isWeekend(this.contextMenu.targetDay) || this.isJourFerie(this.contextMenu.targetDay))) {
      this.showError('Impossible d\'ajouter un shift les jours fériés ou week-ends.');
      this.closeContextMenu();
      return;
    }

    if (!isAdmin && !isManager && this.contextMenu.targetPrestataire) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      if (!userPrestataireId) { this.showError('Impossible de déterminer votre prestataire'); this.closeContextMenu(); return; }
      const targetId = typeof this.contextMenu.targetPrestataire === 'string' ? this.contextMenu.targetPrestataire : this.contextMenu.targetPrestataire.id;
      if (targetId !== userPrestataireId) { this.showError('Vous ne pouvez ajouter que vos propres shifts'); this.closeContextMenu(); return; }
    }

    if (this.contextMenu.targetDay && this.contextMenu.targetPrestataire) {
      this.openCreateModal(this.contextMenu.targetDay, this.contextMenu.targetPrestataire);
    }
    this.closeContextMenu();
  }

  pasteShiftsFromContext(): void {
    if (!this.copiedData || !this.contextMenu.targetDay || !this.contextMenu.targetPrestataire) return;
    const targetDate = this.contextMenu.targetDay;
    const targetPrestataire = this.contextMenu.targetPrestataire;

    // ✅ SÉCURITÉ : un User ne peut coller que dans sa propre ligne
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      const targetId = typeof targetPrestataire === 'string' ? targetPrestataire : targetPrestataire.id;
      if (!userPrestataireId || targetId !== userPrestataireId) {
        this.showError('Vous ne pouvez coller que dans votre propre ligne');
        this.closeContextMenu();
        return;
      }
    }

    if (this.copiedData.dates.length > 1) {
      this.pasteMutipleDays(targetDate, targetPrestataire);
    } else {
      this.pasteSingleDay(targetDate, targetPrestataire);
    }
    this.closeContextMenu();
  }

  deleteShiftFromContext(): void {
    const selectedShiftObjects = this.shifts.filter(s => this.selectedShifts.has(s.id));
    if (selectedShiftObjects.length === 0) return;

    const cannotDelete = selectedShiftObjects.some(shift => !this.canModifyShift(shift));
    if (cannotDelete) {
      this.showError('Vous ne pouvez supprimer que vos propres shifts');
      this.closeContextMenu();
      return;
    }

    const message = selectedShiftObjects.length === 1
      ? `Voulez-vous vraiment supprimer ce shift ?`
      : `Voulez-vous vraiment supprimer ces ${selectedShiftObjects.length} shifts ?`;

    this.showConfirm('Confirmer la suppression', message, () => {
      let successCount = 0;
      let errorCount = 0;
      selectedShiftObjects.forEach(shift => {
        this.shiftService.deleteShift(shift.id).subscribe({
          next: () => { successCount++; if (successCount + errorCount === selectedShiftObjects.length) { this.clearSelection(); this.loadShifts(); } },
          error: () => { errorCount++; if (successCount + errorCount === selectedShiftObjects.length) { this.clearSelection(); this.loadShifts(); } }
        });
      });
    });
    this.closeContextMenu();
  }

  // ========== COPIER/COLLER ==========

  copySelectedShifts(): void {
    const selectedShiftObjects = this.shifts.filter(s => this.selectedShifts.has(s.id));
    if (selectedShiftObjects.length === 0) return;
    const uniqueDates = [...new Set(selectedShiftObjects.map(s => new Date(s.date).toDateString()))].map(d => new Date(d)).sort((a, b) => a.getTime() - b.getTime());
    this.copiedData = { shifts: selectedShiftObjects, dates: uniqueDates, prestataire: selectedShiftObjects[0].prestataireId };
    this.clearSelection();
  }

  private pasteMutipleDays(targetDate: Date, targetPrestataire: Prestataire): void {
    let actualTargetDate = targetDate;
    if (!this.isJourOuvrable(targetDate)) actualTargetDate = this.getNextJourOuvrable(targetDate);

    const numberOfDays = this.copiedData!.dates.length;
    const numberOfShifts = this.copiedData!.shifts.length;
    /* const confirmMsg = `Coller ${numberOfShifts} shift(s) sur ${numberOfDays} jours ouvrables pour ${targetPrestataire.prenom} ${targetPrestataire.nom} à partir du ${actualTargetDate.toLocaleDateString('fr-FR')} ?\n\n⚠️ Les week-ends et jours fériés seront automatiquement exclus.`;
    if (!confirm(confirmMsg)) return; */

    let successCount = 0, errorCount = 0, skippedCount = 0;
    const totalOperations = numberOfShifts;
    const firstCopiedDate = this.copiedData!.dates[0];

    this.copiedData!.shifts.forEach(shift => {
      const shiftDate = new Date(shift.date);
      const daysDiff = Math.floor((shiftDate.getTime() - firstCopiedDate.getTime()) / (1000 * 60 * 60 * 24));
      let newDate = new Date(actualTargetDate);
      let joursOuvrablesComptes = 0;
      while (joursOuvrablesComptes < daysDiff) {
        newDate.setDate(newDate.getDate() + 1);
        if (this.isJourOuvrable(newDate)) joursOuvrablesComptes++;
      }
      if (!this.isJourOuvrable(newDate)) { skippedCount++; errorCount++; return; }

      this.shiftService.createShift(targetPrestataire.id!, newDate, shift.typeShift.id, shift.notes).subscribe({
        next: () => { successCount++; if (successCount + errorCount === totalOperations) { this.loadShifts(); this.showSuccess(`✅ ${successCount} shift(s) collé(s) avec succès !${skippedCount > 0 ? ` (${skippedCount} ignoré(s))` : ''}`); } },
        error: () => { errorCount++; if (successCount + errorCount === totalOperations) { this.loadShifts(); this.showWarning(`⚠️ ${successCount} shift(s) collé(s), ${errorCount - skippedCount} erreur(s)`); } }
      });
    });
  }

  private pasteSingleDay(targetDate: Date, targetPrestataire: Prestataire): void {
    if (!this.isJourOuvrable(targetDate)) {
      const nextOuvrable = this.getNextJourOuvrable(targetDate);
      const confirmMsg = `⚠️ Le ${targetDate.toLocaleDateString('fr-FR')} n'est pas un jour ouvrable.\n\nVoulez-vous coller le prochain jour ouvrable (${nextOuvrable.toLocaleDateString('fr-FR')}) ?`;
      if (!confirm(confirmMsg)) { this.showWarning('❌ Collage annulé'); return; }
      targetDate = nextOuvrable;
    }

    const numberOfShifts = this.copiedData!.shifts.length;
    //if (!confirm(`Coller ${numberOfShifts} shift(s) pour ${targetPrestataire.prenom} ${targetPrestataire.nom} le ${targetDate.toLocaleDateString('fr-FR')} ?`)) return;

    let successCount = 0, errorCount = 0;
    this.copiedData!.shifts.forEach(shift => {
      this.shiftService.createShift(targetPrestataire.id!, targetDate, shift.typeShift.id, shift.notes).subscribe({
        next: () => { successCount++; if (successCount + errorCount === numberOfShifts) { this.loadShifts(); this.showSuccess(`✅ ${successCount} shift(s) collé(s) avec succès !`); } },
        error: () => { errorCount++; if (successCount + errorCount === numberOfShifts) { this.loadShifts(); if (successCount > 0) this.showWarning(`⚠️ ${successCount} collé(s), ${errorCount} erreur(s)`); else this.showError('❌ Erreur lors du collage'); } }
      });
    });
  }

  clearSelection(): void { this.selectedShifts.clear(); }
  clearClipboard(): void { this.copiedData = null; }

  // ========== RACCOURCIS CLAVIER ==========

  setupKeyboardListeners(): void {}

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const tag = (document.activeElement?.tagName ?? '').toLowerCase();
    if (tag === 'input' || tag === 'textarea' || tag === 'select') return;
    if ((e.ctrlKey || e.metaKey) && e.key === 'c') {
      if (this.selectedShifts.size > 0) { e.preventDefault(); this.copySelectedShifts(); }
    }
    if ((e.ctrlKey || e.metaKey) && e.key === 'v') {
      if (this.copiedData && this.selectedCell) { e.preventDefault(); this.pasteToSelectedCell(); }
    }
    if (e.key === 'Delete') {
      if (this.selectedShifts.size > 0) { e.preventDefault(); this.deleteShiftFromContext(); }
    }
    if (e.key === 'Escape') {
      this.clearSelection(); this.closeContextMenu(); this.selectedCell = null; this.cdr.detectChanges();
    }
  }

  initDragSelect(event: MouseEvent, container: HTMLElement): void {
    if (event.button !== 0) return;
    const target = event.target as HTMLElement;
    if (target.closest('.shift-badge') || target.closest('button') || target.closest('.prestataire-cell') || target.closest('.prestataire-header')) return;
    event.preventDefault();
    this.dragContainer = container;
    const r = container.getBoundingClientRect();
    this.dragStart = { x: event.clientX - r.left + container.scrollLeft, y: event.clientY - r.top + container.scrollTop };
    this.isDragging = false;
    this.dragRect = null;
    const onMove = (e: MouseEvent) => {
      if (!this.dragStart || !this.dragContainer) return;
      const cr = this.dragContainer.getBoundingClientRect();
      const cx = e.clientX - cr.left + this.dragContainer.scrollLeft;
      const cy = e.clientY - cr.top + this.dragContainer.scrollTop;
      const dx = cx - this.dragStart.x, dy = cy - this.dragStart.y;
      if (!this.isDragging && Math.abs(dx) < 4 && Math.abs(dy) < 4) return;
      this.isDragging = true;
      this.dragRect = { left: Math.min(this.dragStart.x, cx), top: Math.min(this.dragStart.y, cy), width: Math.abs(dx), height: Math.abs(dy) };
      this.cdr.detectChanges();
    };
    const onUp = () => {
      document.removeEventListener('mousemove', onMove);
      document.removeEventListener('mouseup', onUp);
      if (this.isDragging && this.dragRect && this.dragContainer) {
        const badges = this.dragContainer.querySelectorAll<HTMLElement>('.shift-badge[data-shift-id]');
        const sel = new Set<string>();
        const cr = this.dragContainer.getBoundingClientRect();
        badges.forEach(b => {
          const br = b.getBoundingClientRect();
          const bl = br.left - cr.left + this.dragContainer!.scrollLeft;
          const bt = br.top - cr.top + this.dragContainer!.scrollTop;
          if (bl < this.dragRect!.left + this.dragRect!.width && bl + br.width > this.dragRect!.left &&
              bt < this.dragRect!.top + this.dragRect!.height && bt + br.height > this.dragRect!.top) {
            const id = b.dataset['shiftId']; if (id) sel.add(id);
          }
        });
        if (sel.size > 0) this.selectedShifts = sel;
      }
      this.isDragging = false; this.dragStart = null; this.dragRect = null; this.dragContainer = null;
      this.cdr.detectChanges();
    };
    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
  }

  // ========== MODAL ==========

  openCreateModal(day: Date, prestataire: Prestataire): void {
    this.modalMode = 'create';
    this.selectedDate = day;
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      this.selectedPrestataire = userPrestataireId || prestataire.id!;
    } else {
      this.selectedPrestataire = prestataire.id!;
    }
    this.selectedTypeShiftId = '';
    this.shiftNotes = '';
    this.isDateRangeEnabled = false;
    this.rangeStartDate = this.formatDateForInput(day);
    this.rangeEndDate = this.formatDateForInput(day);
    this.showModal = true;
  }

  openEditModal(shift: Shift): void {
    this.modalMode = 'edit';
    this.selectedShift = shift;
    this.selectedTypeShiftId = shift.typeShift.id;
    this.shiftNotes = shift.notes || '';
    this.nombreJours = shift.nombreJours || 1;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedShift = null;
    this.selectedDate = null;
    this.selectedPrestataire = '';
    this.selectedTypeShiftId = '';
    this.shiftNotes = '';
    this.nombreJours = 1;
    this.isDateRangeEnabled = false;
    this.cdr.detectChanges();
  }

  saveShift(): void {
    if (this.modalMode === 'create') this.createShift();
    else this.updateShift();
  }

  private createShift(): void {
    if (!this.selectedTypeShiftId || !this.selectedPrestataire || !this.selectedDate) return;
    if (this.isDateRangeEnabled && this.rangeStartDate && this.rangeEndDate) {
      this.createMultipleShifts();
    } else {
      this.createSingleShift();
    }
  }

  private createSingleShift(): void {
    this.shiftService.createShift(this.selectedPrestataire, this.selectedDate!, this.selectedTypeShiftId, this.shiftNotes, this.nombreJours).subscribe({
      next: () => { this.loadShifts(); this.closeModal(); this.showSuccess('✅ Shift ajouté avec succès !'); },
      error: () => this.showError('❌ Erreur lors de l\'ajout du shift')
    });
  }

  private createMultipleShifts(): void {
    const startDate = new Date(this.rangeStartDate);
    const endDate = new Date(this.rangeEndDate);
    const dates: Date[] = [];
    const skippedDates: Date[] = [];
    const currentDate = new Date(startDate);

    while (currentDate <= endDate) {
      const dateToCheck = new Date(currentDate);
      if (this.isJourOuvrable(dateToCheck)) dates.push(new Date(currentDate));
      else skippedDates.push(new Date(currentDate));
      currentDate.setDate(currentDate.getDate() + 1);
    }

    if (dates.length === 0) { this.showWarning('⚠️ Aucun jour ouvrable dans cet intervalle'); return; }

    let successCount = 0, errorCount = 0;
    dates.forEach(date => {
      this.shiftService.createShift(this.selectedPrestataire, date, this.selectedTypeShiftId, this.shiftNotes, this.nombreJours).subscribe({
        next: () => {
          successCount++;
          if (successCount + errorCount === dates.length) {
            this.loadShifts(); this.closeModal();
            let message = `✅ ${successCount} shift(s) ajouté(s) avec succès !`;
            if (skippedDates.length > 0) message += ` (${skippedDates.length} jour(s) exclu(s))`;
            if (errorCount === 0) this.showSuccess(message); else this.showWarning(`⚠️ ${successCount} ajouté(s), ${errorCount} erreur(s)`);
          }
        },
        error: () => { errorCount++; if (successCount + errorCount === dates.length) { this.loadShifts(); this.closeModal(); if (successCount === 0) this.showError('❌ Erreur lors de l\'ajout'); else this.showWarning(`⚠️ ${successCount} ajouté(s), ${errorCount} erreur(s)`); } }
      });
    });
  }

  private updateShift(): void {
    if (!this.selectedShift || !this.selectedTypeShiftId) return;
    this.shiftService.updateShift(this.selectedShift.id, this.selectedTypeShiftId, this.shiftNotes, this.nombreJours).subscribe({
      next: () => { this.loadShifts(); this.closeModal(); this.showSuccess('✅ Shift modifié avec succès !'); },
      error: () => this.showError('❌ Erreur lors de la modification du shift')
    });
  }

  deleteShift(shift: Shift, event: Event): void {
    event.stopPropagation();
    this.showConfirm('Confirmer la suppression', `Voulez-vous vraiment supprimer le shift "${shift.typeShift.label}" ?`, () => {
      this.shiftService.deleteShift(shift.id).subscribe({
        next: () => { this.loadShifts(); this.showSuccess('✅ Shift supprimé avec succès !'); },
        error: () => this.showError('❌ Erreur lors de la suppression du shift')
      });
    });
  }

  // ========== ✅ IMAGE AVATAR ==========
  getAvatarUrl(prestataire: Prestataire): string | null {
    if (!prestataire.imageUrl) return null;
    if (prestataire.imageUrl.startsWith('http')) return prestataire.imageUrl;
    const base = (environment as any).apiUrl?.replace('/api', '') || '';
    return `${base}/${prestataire.imageUrl}`;
  }

  // ========== ✅ RÔLE HELPERS ==========
  isAdminOrManager(): boolean {
    return this.permissionService.hasRole(Role.Admin) || this.permissionService.hasRole(Role.Manager);
  }

  isUserRole(): boolean {
    return !this.isAdminOrManager();
  }

  // ========== ✅ CELLULE SÉLECTIONNÉE (position) ==========
  selectedCell: { day: Date; prestataire: Prestataire } | null = null;

  onCellSingleClick(event: MouseEvent, day: Date, prestataire: Prestataire): void {
    const target = event.target as HTMLElement;
    if (target.closest('.shift-badge')) return;

    // Simple clic sur la cellule = mémoriser uniquement la position pour Ctrl+V
    // ⚠️ NE PAS sélectionner les shifts (le badge ✓ n'apparaît que via Ctrl+Clic sur le shift)
    this.selectedCell = { day, prestataire };
    this.selectedShifts.clear();   // ← désélectionner tout
    this.cdr.detectChanges();
  }

  onCellDblClick(event: MouseEvent, day: Date, prestataire: Prestataire): void {
    // Double clic = ouvrir le modal de création
    event.stopPropagation();
    const target = event.target as HTMLElement;
    if (target.closest('.shift-badge')) return;
    this.selectedCell = { day, prestataire };
    this.onCellClick(event, day, prestataire);
  }

  pasteToSelectedCell(): void {
    if (!this.copiedData || !this.selectedCell) return;

    // ✅ SÉCURITÉ : un User ne peut coller que dans sa propre ligne (Ctrl+V)
    const isAdmin = this.permissionService.hasRole(Role.Admin);
    const isManager = this.permissionService.hasRole(Role.Manager);
    if (!isAdmin && !isManager) {
      const userPrestataireId = this.getCurrentUserPrestataireId();
      if (!userPrestataireId || this.selectedCell.prestataire.id !== userPrestataireId) {
        this.showError('Vous ne pouvez coller que dans votre propre ligne');
        return;
      }
    }

    this.pasteShiftsTo(this.selectedCell.day, this.selectedCell.prestataire);
  }

  private pasteShiftsTo(targetDate: Date, targetPrestataire: Prestataire): void {
    if (this.copiedData!.dates.length > 1) {
      this.pasteMutipleDays(targetDate, targetPrestataire);
    } else {
      this.pasteSingleDay(targetDate, targetPrestataire);
    }
  }

  isCellSelected(day: Date, prestataireId: string): boolean {
    if (!this.selectedCell) return false;
    const d = this.selectedCell.day;
    return this.selectedCell.prestataire.id === prestataireId &&
           d.getDate() === day.getDate() &&
           d.getMonth() === day.getMonth() &&
           d.getFullYear() === day.getFullYear();
  }

  getSelectedCellLabel(): string {
    if (!this.selectedCell) return '';
    const d = this.selectedCell.day;
    const dateStr = d.toLocaleDateString('fr-FR', { weekday: 'short', day: 'numeric', month: 'short' });
    return `📍 ${this.selectedCell.prestataire.prenom} ${this.selectedCell.prestataire.nom} — ${dateStr}`;
  }

  // ========== UTILITAIRES ==========

  getInitials(prestataireId: string): string {
    const prestataire = this.prestataires.find(p => p.id === prestataireId);
    if (!prestataire) return '??';
    return (prestataire.prenom?.charAt(0).toUpperCase() || '') + (prestataire.nom?.charAt(0).toUpperCase() || '');
  }

  getPrestataireLabel(prestataireId: string): string {
    const prestataire = this.prestataires.find(p => p.id === prestataireId);
    return prestataire ? `${prestataire.prenom} ${prestataire.nom}` : '';
  }

  private formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private formatMonthForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    return `${year}-${month}`;
  }

  // ==================== MODAL CONFIRMATION ====================

  showConfirm(title: string, message: string, action: () => void): void {
    this.confirmTitle = title;
    this.confirmMessage = message;
    this.confirmAction = action;
    this.showConfirmModal = true;
  }

  confirmDelete(): void {
    if (this.confirmAction) this.confirmAction();
    this.closeConfirmModal();
  }

  closeConfirmModal(): void {
    this.showConfirmModal = false;
    this.confirmTitle = '';
    this.confirmMessage = '';
    this.confirmAction = null;
  }

  // ==================== NOTIFICATIONS ====================

  removeNotification(_id: number): void { /* géré par NotificationService */ }
  showSuccess(message: string, _autoClose = true): void { this.notifService.success(message); }
  showError(message: string, _autoClose = true): void { this.notifService.error(message); }
  showWarning(message: string, _autoClose = true): void { this.notifService.warning(message); }
  showInfo(message: string, _autoClose = true): void { this.notifService.info(message); }
  closeErrorModal(): void { this.showErrorModal = false; this.errorMessage = ''; }

  // ✅ Style inline basé sur la couleur hex — fallback quand les classes CSS sont absentes
  getShiftStyle(typeShift: any): { [key: string]: string } {
    if (!typeShift?.color) return {};
    const hex = typeShift.color;
    // Si pas de classes CSS définies → utiliser le hex directement
    const hasClasses = typeShift.bgColor || typeShift.borderColor || typeShift.textColor;
    if (hasClasses) return {}; // les classes gèrent tout
    // Calculer si la couleur est claire ou foncée pour le texte
    const r = parseInt(hex.slice(1, 3), 16) || 0;
    const g = parseInt(hex.slice(3, 5), 16) || 0;
    const b = parseInt(hex.slice(5, 7), 16) || 0;
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    return {
      'background-color': hex,
      'border-color': hex,
      'color': luminance > 0.5 ? '#1a1a1a' : '#ffffff',
      'border-width': '2px',
      'border-style': 'solid'
    };
  }
}