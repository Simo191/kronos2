import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ContratService, ContratDto, RepartitionAnnuelle } from '../../services/contrat.service';
import { PlanningService } from '../../services/planning.service';
import { ApplicationPlanning } from '../../models/planning.model';
import { PrestataireService } from '../../services/prestataire.service';
import { TypeContratService, TypeContratDto } from '../../services/type-contrat.service';
import { JourTravailService } from '../../services/jour-travail.service';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';
//  AJOUT PERMISSIONS
import { PermissionService, Permission, Role } from '../../services/permission.service';
import { HasPermissionDirective } from '../../directives/permission.directives';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';

@Component({
  selector: 'app-contrats',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FormatMontantPipe,
    HasPermissionDirective //  AJOUT
    ,
    NotificationToastComponent
],
  templateUrl: './contrats.component.html',
  styleUrls: ['./contrats.component.scss']
})
export class ContratsComponent implements OnInit {
  contrats: ContratDto[] = [];
  filteredContrats: ContratDto[] = [];
  prestataires: any[] = [];
  //   Toggle "Inclure désactivés" — partagé entre filtre et sélecteur
  showInactivePrest = false;
  typesContrat: TypeContratDto[] = [];
  selectedContrat: Partial<ContratDto> = {};
  allJoursTravail: any[] = []; //  Pour calculer jours utilisés

  // ─── Congés Payés ──────────────────────────────────────────────────────────
  joursOuvresBruts = 0;   // Jours ouvrés bruts avant déduction CP
  congesCP = 0;           // CP saisis manuellement
  cpProRataAuto = 0;      // CP calculé pro-rata (25 CP / 228 jours/an)
  // ───────────────────────────────────────────────────────────────────────────

  //  UPLOAD DOCUMENT
  selectedFile: File | null = null;
  isUploading = false;

  // ─── Suivi des commandes ───────────────────────────────────────────────────
  applications: ApplicationPlanning[] = [];   // Logiciels (applications du planning)
  repartitionEnCours = false;
  repartitionModifiee = false;                // true = saisie manuelle après la proposition auto
  repartitionEnregistree = false;             // true = répartition chargée depuis le contrat (déjà saisie)
  private repartitionRequete = 0;             // n° de la dernière demande (ignore les réponses obsolètes)
  // ───────────────────────────────────────────────────────────────────────────
   
  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  viewMode: 'list' | 'cards' = 'cards';
  
  // Filtres
  searchTerm = '';
  filterPrestataire = '';
  filterTypeContrat = '';
  filterStatut = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 10;
  
  // Tri
  sortColumn: string = 'prestataire';
  sortDirection: 'asc' | 'desc' = 'asc';
  
  // Statistiques
  totalContrats = 0;
  contratsActifs = 0;
  tjmMoyen = 0;
  tjmMax = 0;
  tjmMin = 0;
  
  // Notification
  
  // Confirmation suppression
  showDeleteConfirm = false;
  contratToDelete: ContratDto | null = null;

  //  AJOUT PERMISSIONS
  Permission = Permission;
  Role = Role;

  constructor(
    private contratService: ContratService,
    private prestataireService: PrestataireService,
    private typeContratService: TypeContratService,
    private jourTravailService: JourTravailService,
    private cdr: ChangeDetectorRef,
    private notifService: NotificationService,
    private permissionService: PermissionService,
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,   //  UPLOAD
    private planningService: PlanningService
  ) {}

  // ========== CALCUL AUTOMATIQUE JOURS FÉRIÉS ==========
  
  //  Algorithme de Computus (calcul de la date de Pâques)
  private calculateEasterDate(year: number): Date {
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
    const month = Math.floor((h + l - 7 * m + 114) / 31);
    const day = ((h + l - 7 * m + 114) % 31) + 1;
    
    return new Date(year, month - 1, day);
  }

  //  Générer tous les jours fériés pour une année donnée
  private getJoursFeriesForYear(year: number): Date[] {
    const feries: Date[] = [];

    // Jours fériés FIXES
    feries.push(new Date(year, 0, 1));   // 01/01 - Jour de l'an
    feries.push(new Date(year, 4, 1));   // 01/05 - Fête du travail
    feries.push(new Date(year, 4, 8));   // 08/05 - Victoire 1945
    feries.push(new Date(year, 6, 14));  // 14/07 - Fête nationale
    feries.push(new Date(year, 7, 15));  // 15/08 - Assomption
    feries.push(new Date(year, 10, 1));  // 01/11 - Toussaint
    feries.push(new Date(year, 10, 11)); // 11/11 - Armistice 1918
    feries.push(new Date(year, 11, 25)); // 25/12 - Noël

    // Jours fériés MOBILES (basés sur Pâques)
    const paques = this.calculateEasterDate(year);
    
    // Lundi de Pâques (Pâques + 1 jour)
    const lundiPaques = new Date(paques);
    lundiPaques.setDate(paques.getDate() + 1);
    feries.push(lundiPaques);
    
    // Ascension (Pâques + 39 jours)
    const ascension = new Date(paques);
    ascension.setDate(paques.getDate() + 39);
    feries.push(ascension);
    
    // Lundi de Pentecôte (Pâques + 50 jours)
    const pentecote = new Date(paques);
    pentecote.setDate(paques.getDate() + 50);
    feries.push(pentecote);

    return feries;
  }

  //  Vérifier si une date est un jour férié
  private isJourFerie(date: Date): boolean {
    const year = date.getFullYear();
    const feries = this.getJoursFeriesForYear(year);
    
    return feries.some(ferie => 
      ferie.getDate() === date.getDate() &&
      ferie.getMonth() === date.getMonth() &&
      ferie.getFullYear() === date.getFullYear()
    );
  }

  private formatDateForInput(dateStr: string | undefined): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return this.formatDateISO(date);
  }

  private formatDateISO(date: Date): string {
    const yyyy = date.getFullYear();
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  //  CALCUL AUTOMATIQUE DES JOURS OUVRÉS
  calculateJoursOuvres(): void {
    if (!this.selectedContrat.dateDebut || !this.selectedContrat.dateFin) {
      this.selectedContrat.nombreJours = 0;
      this.joursOuvresBruts = 0;
      this.cpProRataAuto = 0;
      return;
    }

    const dateDebut = new Date(this.selectedContrat.dateDebut);
    const dateFin = new Date(this.selectedContrat.dateFin);

    if (dateFin < dateDebut) {
      this.selectedContrat.nombreJours = 0;
      this.joursOuvresBruts = 0;
      this.cpProRataAuto = 0;
      return;
    }

    // ── 1. Compter les jours ouvrés bruts ───────────────────────────────────
/*     let joursOuvres = 0;
    const currentDate = new Date(dateDebut);
    while (currentDate <= dateFin) {
      const dayOfWeek = currentDate.getDay();
      const isWeekend = dayOfWeek === 0 || dayOfWeek === 6;
      const isFerie = this.isJourFerie(currentDate);
      if (!isWeekend && !isFerie) joursOuvres++;
      currentDate.setDate(currentDate.getDate() + 1);
    }
    this.joursOuvresBruts = joursOuvres;

    // ── 2. Calculer le CP pro-rata automatique (25 CP légaux / 228 jours/an) ─
    this.cpProRataAuto = Math.round(joursOuvres * (25 / 228) * 10) / 10;
    this.cpProRataAuto = this.cpProRataAuto > 25 ? 25 : this.cpProRataAuto ;
    // ── 3. Appliquer les CP saisis ──────────────────────────────────────────
    this.selectedContrat.nombreJours = Math.max(0, joursOuvres - (this.congesCP || 0)); */
  }

/*   // Déclenché quand l'utilisateur modifie le champ CP
  onCongesCPChange(): void {
    this.selectedContrat.nombreJours = Math.max(0, this.joursOuvresBruts - (this.congesCP || 0));
  } */

  // Applique le pro-rata automatique dans le champ CP
/*   applyProRataCP(): void {
    this.congesCP = this.cpProRataAuto;
    this.onCongesCPChange();
  } */

  //  Recalcule nombreJours depuis montantTotal / tjm
  calculateNombreJoursFromMontant(): void {
    const montant = Number(this.selectedContrat.montantTotal || 0);
    const tjm = Number(this.selectedContrat.tjm || 0);
    if (tjm > 0) {
      this.selectedContrat.nombreJours = Math.round(montant / tjm);
    }
  }

  //  Appelé quand montantTotal change (saisie manuelle)
  onMontantTotalChange(): void {
    this.calculateNombreJoursFromMontant();
    this.proposerRepartition();
  }

  //  Appelé quand les dates changent dans le formulaire
  onDateChange(): void {
    this.calculateJoursOuvres();
    this.proposerRepartition();
  }

  //  Appelé quand le TJM change
  onTjmChange(): void {
    this.calculateNombreJoursFromMontant();
    this.proposerRepartition();
  }

  // ========== SUIVI DES COMMANDES : LOGICIEL + RÉPARTITION ANNUELLE ==========

  loadApplications(): void {
    this.planningService.getAll().subscribe({
      next: (data) => {
        this.applications = data ?? [];
        this.cdr.detectChanges();
      },
      error: () => this.cdr.detectChanges()
    });
  }

  //  Logiciels où le prestataire sélectionné est affecté dans le planning (proposés en premier)
  get applicationsAffectees(): ApplicationPlanning[] {
    const pid = this.selectedContrat.prestataireId;
    if (!pid) return [];
    return this.applications.filter(a => (a.prestataires ?? []).some(p => p.prestataireId === pid));
  }

  get applicationsAutres(): ApplicationPlanning[] {
    const affectees = new Set(this.applicationsAffectees.map(a => a.id));
    return this.applications.filter(a => !affectees.has(a.id));
  }

  //  Sans logiciel choisi, le suivi des commandes utilise celui du planning : on garde null
  //  (le libellé de l'option "Selon le planning" indique le logiciel qui sera retenu)
  onPrestataireFormChange(): void {
    this.cdr.detectChanges();
  }

  get libelleLogicielPlanning(): string {
    const affectees = this.applicationsAffectees;
    if (!affectees.length) return 'Aucun (non affecté dans le planning)';
    return `Selon le planning (${affectees[0].nom}${affectees.length > 1 ? ` + ${affectees.length - 1} autre(s)` : ''})`;
  }

  //  Répartition automatique (prorata des jours ouvrés par année), modifiable ensuite
  proposerRepartition(): void {
    const { dateDebut, dateFin } = this.selectedContrat;
    const nombreJours = Number(this.selectedContrat.nombreJours || 0);
    const debut = dateDebut ? new Date(dateDebut) : null;
    const fin = dateFin ? new Date(dateFin) : null;
    //  Période exploitable uniquement (évite les dates en cours de saisie, ex. année 0002)
    const periodeValide = !!debut && !!fin && !isNaN(debut.getTime()) && !isNaN(fin.getTime())
      && fin >= debut && debut.getFullYear() >= 2000 && fin.getFullYear() - debut.getFullYear() <= 10;

    const requete = ++this.repartitionRequete;
    this.repartitionEnregistree = false;
    if (!periodeValide || nombreJours <= 0) {
      this.selectedContrat.repartitions = [];
      this.repartitionModifiee = false;
      this.repartitionEnCours = false;
      return;
    }

    this.repartitionEnCours = true;
    this.contratService.getRepartitionProposee(dateDebut!, dateFin!, nombreJours).subscribe({
      next: (proposition) => {
        if (requete !== this.repartitionRequete) return;   // réponse obsolète
        this.selectedContrat.repartitions = proposition ?? [];
        this.repartitionModifiee = false;
        this.repartitionEnCours = false;
        this.cdr.detectChanges();
      },
      error: () => {
        if (requete !== this.repartitionRequete) return;
        this.repartitionEnCours = false;
        this.cdr.detectChanges();
      }
    });
  }

  get totalRepartition(): number {
    return (this.selectedContrat.repartitions ?? [])
      .reduce((sum, r) => sum + Number(r.nombreJours || 0), 0);
  }

  //  Écart entre la répartition saisie et le nombre de jours du contrat (0 = OK)
  get ecartRepartition(): number {
    if (!(this.selectedContrat.repartitions ?? []).length) return 0;
    return Math.round((this.totalRepartition - Number(this.selectedContrat.nombreJours || 0)) * 100) / 100;
  }

  //  Même règle que le backend : montant au prorata du contrat
  getMontantRepartition(r: RepartitionAnnuelle): number {
    const jours = Number(this.selectedContrat.nombreJours || 0);
    const montant = Number(this.selectedContrat.montantTotal || 0);
    return jours > 0 ? montant * Number(r.nombreJours || 0) / jours : 0;
  }

  ngOnInit() {
    const prestataireId = this.route.snapshot.queryParamMap.get('prestataireId');
  if (prestataireId) {
    this.filterPrestataire = prestataireId;
  }
    this.loadContrats();
    this.loadPrestataires();
    
      if (this.permissionService.hasPermission(Permission.VIEW_TYPE_CONTRAT)) {
    this.loadTypesContrat();
    }
    this.loadJoursTravail(); //  Pour calculer jours restants
    if (this.canManageContrat()) {
      this.loadApplications(); //  Logiciels pour le suivi des commandes
    }
  }

  //  MODIFIÉ : Filtrer contrats selon rôle
  loadContrats() {
    this.isLoading = true;
    this.contratService.getAll().subscribe({
      next: (data) => {
        
        //  Filtrer selon rôle
        if (this.permissionService.hasRole(Role.User)) {
          // User : seulement ses contrats
          const userPrestataireId = this.getCurrentUserPrestataireId();
          this.contrats = data.filter(c => c.prestataire?.id === userPrestataireId).sort((s,b) =>s.prestataire?.prenom - b.prestataire?.nom);
        } else {
          // Admin/Manager : tous les contrats
          this.contrats = data ?? [];
        }
        
        //  Réappliquer les filtres actifs au lieu de tout afficher
        this.isLoading = false;
        this.applyFilters();
        
        this.cdr.detectChanges();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du chargement des contrats');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadPrestataires() {
    this.prestataireService.getAll(this.showInactivePrest).subscribe({
      next: (data) => {
        this.prestataires = (data ?? []).filter(f=>f.responsable !== true);
        this.cdr.detectChanges();
      },
      error: () => {
        this.cdr.detectChanges();
      }
    });
  }

  //   Bascule l'inclusion des désactivés (rechargement)
  toggleShowInactivePrest(): void {
    this.showInactivePrest = !this.showInactivePrest;
    //   Nettoie le filtre si on retire un inactif sélectionné
    if (!this.showInactivePrest && (this as any).filterPrestataire) {
      const sel = this.prestataires.find((p: any) => p.id === (this as any).filterPrestataire);
      if (sel?.isActive === false) (this as any).filterPrestataire = '';
    }
    this.loadPrestataires();
  }

  loadTypesContrat() {
    this.typeContratService.getAll().subscribe({
      next: (data) => {
        this.typesContrat = data ?? [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.cdr.detectChanges();
      }
    });
  }

  loadJoursTravail() {
    this.jourTravailService.getAll().subscribe({
      next: (data) => {
        this.allJoursTravail = data ?? [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.cdr.detectChanges();
      }
    });
  }

  //  Calculer les jours utilisés pour un contrat
  getJoursUtilises(contrat: ContratDto): number {
    if (!this.allJoursTravail || this.allJoursTravail.length === 0) {
      return 0;
    }

    const contratDebut = new Date(contrat.dateDebut);
    const contratFin   = contrat.dateFin ? new Date(contrat.dateFin) : new Date();

    return this.allJoursTravail
      .filter(j => j.prestataireId === contrat.prestataireId)
      .filter(j => {
        // Chevauchement mois/contrat (évite le bug du jour 15 avant dateDebut)
        const [year, month] = j.mois.split('-').map(Number);
        const moisDebut = new Date(year, month - 1, 1);
        const moisFin   = new Date(year, month, 0, 23, 59, 59);
        return moisDebut <= contratFin && moisFin >= contratDebut;
      })
      .reduce((sum, j) => sum + (j.nombreJours || 0), 0);
  }

  //  Calculer les jours restants pour un contrat
  getJoursRestants(contrat: ContratDto): number {
    const quotaTotal = contrat.nombreJours || 0;
    const joursUtilises = this.getJoursUtilises(contrat);
    return Math.max(0, quotaTotal - joursUtilises);
  }

  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  //  MODIFIÉ : Stats selon permission
  calculateStats() {
    const contratsACalculer = this.filteredContrats.length > 0 ? this.filteredContrats : this.contrats;
    
    this.totalContrats = contratsACalculer.length;
    this.contratsActifs = contratsACalculer.filter(c => this.isContratActif(c)).length;

    //  Calculer TJM seulement si permission
    if (this.canViewTauxJournalier()) {
      const tjms = contratsACalculer.map(c => c.tjm || 0);
      const totalTjm = tjms.reduce((sum, tjm) => sum + tjm, 0);
      
      if (tjms.length > 0) {
        this.tjmMoyen = Math.round(totalTjm / tjms.length);
        this.tjmMax = Math.max(...tjms);
        this.tjmMin = Math.min(...tjms);
      } else {
        this.tjmMoyen = 0;
        this.tjmMax = 0;
        this.tjmMin = 0;
      }
    } else {
      // User : masquer TJM
      this.tjmMoyen = 0;
      this.tjmMax = 0;
      this.tjmMin = 0;
    }
  }

  // Filtres
  applyFilters() {
    this.filteredContrats = this.contrats.filter(contrat => {
      const searchMatch = !this.searchTerm ||
        contrat.prestataire?.nom?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        contrat.prestataire?.prenom?.toLowerCase().includes(this.searchTerm.toLowerCase());

      const prestataireMatch = !this.filterPrestataire ||
        contrat.prestataireId === this.filterPrestataire;

      const typeMatch = !this.filterTypeContrat ||
        contrat.typeContratId === this.filterTypeContrat;

      let statutMatch = true;
      if (this.filterStatut === 'actif') statutMatch = this.isContratActif(contrat);
      else if (this.filterStatut === 'termine') statutMatch = !this.isContratActif(contrat);

      return searchMatch && prestataireMatch && typeMatch && statutMatch;
    });

    this.currentPage = 1;
    this.calculateStats(); //  Recalculer les stats après filtrage
    this.applySorting(); //  Appliquer le tri
  }

  // ========== TRI ==========
  
  onSortChange() {
    if (this.sortColumn) {
      this.applySorting();
    }
  }

  toggleSortDirection() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    if (this.sortColumn) {
      this.applySorting();
    }
  }

  sortBy(column: string) {
    if (this.sortColumn === column) {
      // Inverser la direction si même colonne
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Nouvelle colonne : tri ascendant par défaut
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.applySorting();
  }

  applySorting() {
    if (!this.sortColumn) return;

    this.filteredContrats.sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (this.sortColumn) {
        case 'prestataire':
          valueA = (a.prestataire?.nom || '') + ' ' + (a.prestataire?.prenom || '');
          valueB = (b.prestataire?.nom || '') + ' ' + (b.prestataire?.prenom || '');
          break;
        case 'type':
          valueA = a.typeContrat?.nom || a.typeContrat?.libelle || '';
          valueB = b.typeContrat?.nom || b.typeContrat?.libelle || '';
          break;
        case 'dateDebut':
          valueA = new Date(a.dateDebut).getTime();
          valueB = new Date(b.dateDebut).getTime();
          break;
        case 'dateFin':
          valueA = a.dateFin ? new Date(a.dateFin).getTime() : 0;
          valueB = b.dateFin ? new Date(b.dateFin).getTime() : 0;
          break;
        case 'nombreJours':
          valueA = a.nombreJours || 0;
          valueB = b.nombreJours || 0;
          break;
        case 'tjm':
          valueA = a.tjm ||  0;
          valueB = b.tjm ||  0;
          break;
        default:
          return 0;
      }

      // Comparaison
      let comparison = 0;
      if (typeof valueA === 'string') {
        comparison = valueA.localeCompare(valueB, 'fr', { sensitivity: 'base' });
      } else {
        comparison = valueA - valueB;
      }

      return this.sortDirection === 'asc' ? comparison : -comparison;
    });
  }

  getSortIcon(column: string): string {
    if (this.sortColumn !== column) return 'bi-arrow-down-up';
    return this.sortDirection === 'asc' ? 'bi-arrow-up' : 'bi-arrow-down';
  }

  isSorted(column: string): boolean {
    return this.sortColumn === column;
  }

  // Statut contrat
  isContratActif(contrat: ContratDto): boolean {
    if (!contrat.dateFin) return true;
    const today = new Date();
    const dateFin = new Date(contrat.dateFin);
    return dateFin >= today;
  }

  //  MODIFIÉ : Modal avec vérification permission
  openModal(contrat?: ContratDto) {
    //  Vérifier permission
    if (contrat && !this.canManageContrat()) {
      this.showNotification('error', 'Vous ne pouvez pas modifier les contrats');
      return;
    }

    if (contrat) {
      this.isEditMode = true;
      this.selectedContrat = {
        ...contrat,
        dateDebut: this.formatDateForInput(contrat.dateDebut),
        dateFin: contrat.dateFin ? this.formatDateForInput(contrat.dateFin) : undefined,
        tranche: contrat.tranche ?? 'Ferme',
        applicationId: contrat.applicationId ?? null,
        repartitions: (contrat.repartitions ?? []).map(r => ({ ...r }))
      };
      // Reset CP à chaque ouverture
      this.congesCP = 0;
      this.joursOuvresBruts = 0;
      this.cpProRataAuto = 0;
      //  Calculer les jours ouvrés au chargement (mode édition)
      this.calculateJoursOuvres();
      //  Répartition jamais saisie → proposition automatique
      this.repartitionModifiee = false;
      this.repartitionEnregistree = !!this.selectedContrat.repartitions?.length;
      if (!this.selectedContrat.repartitions?.length) {
        this.proposerRepartition();
      }
    } else {
      this.isEditMode = false;
      this.congesCP = 0;
      this.joursOuvresBruts = 0;
      this.cpProRataAuto = 0;
      this.repartitionModifiee = false;
      this.repartitionEnregistree = false;
      this.selectedContrat = {
        nombreJours: 0, //  Sera calculé automatiquement
        tjm: 0,
        tranche: 'Ferme',
        applicationId: null,
        repartitions: []
      };
    }
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedContrat = {};
    this.selectedFile = null; //  Reset fichier
    this.congesCP = 0;
    this.joursOuvresBruts = 0;
    this.cpProRataAuto = 0;
  }

  //  Vérifier chevauchement de contrats pour un prestataire
  checkChevauchementContrats(prestataireId: string, dateDebut: Date, dateFin: Date, contratId?: string): boolean {
    return this.contrats.some(c => {
      // Ignorer le contrat en cours de modification
      if (contratId && c.id === contratId) {
        return false;
      }

      // Vérifier si c'est le même prestataire
      if (c.prestataireId !== prestataireId) {
        return false;
      }

      const cDebut = new Date(c.dateDebut);
      const cFin = c.dateFin ? new Date(c.dateFin) : new Date(2099, 11, 31); // Si pas de fin, date très lointaine

      // Vérifier chevauchement de dates
      // Cas 1 : Le nouveau débute pendant un contrat existant
      const debutDansContrat = dateDebut >= cDebut && dateDebut <= cFin;
      // Cas 2 : Le nouveau se termine pendant un contrat existant
      const finDansContrat = dateFin >= cDebut && dateFin <= cFin;
      // Cas 3 : Le nouveau englobe complètement un contrat existant
      const englobeContrat = dateDebut <= cDebut && dateFin >= cFin;

      return debutDansContrat || finDansContrat || englobeContrat;
    });
  }

  saveContrat() {
    //  VALIDATIONS PROFESSIONNELLES
    if (!this.selectedContrat.prestataireId || this.selectedContrat.prestataireId === '') {
      this.showNotification('warning', 'Veuillez sélectionner un prestataire.');
      return;
    }
    
    if (!this.selectedContrat.typeContratId || this.selectedContrat.typeContratId === '') {
      this.showNotification('warning', 'Veuillez sélectionner un type de contrat.');
      return;
    }
    
    if (!this.selectedContrat.dateDebut) {
      this.showNotification('warning', 'Veuillez saisir la date de début.');
      return;
    }

    //  VALIDATION: Date fin OBLIGATOIRE (pour calcul auto des jours)
    if (!this.selectedContrat.dateFin) {
      this.showNotification('warning', 'Veuillez saisir la date de fin pour calculer automatiquement le nombre de jours.');
      return;
    }

    //  VALIDATION: Date de fin doit être après date de début
    const dateDebut = new Date(this.selectedContrat.dateDebut);
    const dateFin = new Date(this.selectedContrat.dateFin);
    
    if (dateFin < dateDebut) {
      this.showNotification('warning', 'La date de fin ne peut pas être antérieure à la date de début.');
      return;
    }

    //  VALIDATION: Nombre de jours doit être > 0
    if (!this.selectedContrat.nombreJours || this.selectedContrat.nombreJours <= 0) {
      this.showNotification('warning', 'Le nombre de jours doit être supérieur à 0. Vérifiez vos dates.');
      return;
    }

    //  VALIDATION: TJM obligatoire
    if (!this.selectedContrat.tjm || this.selectedContrat.tjm <= 0) {
      this.showNotification('warning', 'Veuillez saisir un TJM valide.');
      return;
    }

    //  VALIDATION: Pas de chevauchement de contrats
    const hasChevauchement = this.checkChevauchementContrats(
      this.selectedContrat.prestataireId,
      dateDebut,
      dateFin,
      this.selectedContrat.id
    );

    if (hasChevauchement) {
      this.showNotification('warning', this.selectedContrat.tranche === 'Option'
        ? 'Cette option chevauche un autre contrat du prestataire. Une tranche option doit commencer après la fin de la tranche ferme.'
        : 'Un contrat existe déjà pour ce prestataire sur cette période. Veuillez vérifier les dates.');
      return;
    }

    //  VALIDATION: la répartition annuelle doit couvrir exactement les jours du contrat
    if (this.repartitionEnCours) {
      this.showNotification('warning', 'Répartition annuelle en cours de calcul, veuillez patienter.');
      return;
    }
    if (this.ecartRepartition !== 0) {
      this.showNotification('warning',
        `La répartition annuelle (${this.totalRepartition} j) doit être égale au nombre de jours du contrat (${this.selectedContrat.nombreJours} j).`);
      return;
    }

    //  Mapper tjm → tauxJournalier pour l'API
    const contratData = {
      ...this.selectedContrat,
      tjm: this.selectedContrat.tjm
    };
    //delete contratData.tjm;
    if (this.isEditMode && this.selectedContrat.id) {
      this.contratService.update(this.selectedContrat.id, contratData as ContratDto).subscribe({
        next: () => {
          if (this.selectedFile && this.selectedContrat.id) {
            this.uploadDocument(this.selectedContrat.id, this.selectedFile, () => {
              this.showNotification('success', 'Contrat modifié et document téléversé avec succès !');
              this.closeModal();
              this.loadContrats();
            });
          } else {
            this.showNotification('success', 'Contrat modifié avec succès !');
            this.closeModal();
            this.loadContrats();
          }
        },
        error: (err) => {
          this.showNotification('error', 'Erreur lors de la modification du contrat.');
        }
      });
    } else {
      this.contratService.create(contratData as ContratDto).subscribe({
        next: (response: any) => {
          const newId = response?.id || response;
          if (this.selectedFile && newId) {
            this.uploadDocument(newId, this.selectedFile, () => {
              this.showNotification('success', 'Contrat modifié et document téléversé avec succès !');
              this.closeModal();
              this.loadContrats();
            });
          } else {
            this.showNotification('success', 'Contrat créé avec succès !');
            this.closeModal();
            this.loadContrats();
          }
        },
        error: (err) => {
          this.showNotification('error', 'Erreur lors de la création du contrat.');
        }
      });
    }
  }

  //  MODIFIÉ : Suppression avec vérification permission
  confirmDelete(contrat: ContratDto) {
    //  Vérifier permission
    if (!this.canManageContrat()) {
      this.showNotification('error', 'Vous ne pouvez pas supprimer les contrats');
      return;
    }

    this.contratToDelete = contrat;
    this.showDeleteConfirm = true;
  }

  cancelDelete() {
    this.showDeleteConfirm = false;
    this.contratToDelete = null;
  }

  deleteContrat() {
    if (this.contratToDelete?.id) {
      this.contratService.delete(this.contratToDelete.id).subscribe({
        next: () => {
          this.showNotification('success', 'Contrat supprimé avec succès !');
          this.loadContrats();
          this.showDeleteConfirm = false;
          this.contratToDelete = null;
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la suppression.');
          this.showDeleteConfirm = false;
          this.contratToDelete = null;
        }
      });
    }
  }

  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notifService.show(type, message);
  }

  // Pagination
  get pagedContrats(): ContratDto[] {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredContrats.slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredContrats.length / this.pageSize);
  }

  get pageNumbers(): number[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const delta = 2;
    const pages: number[] = [];

    for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
      pages.push(i);
    }

    if (current - delta > 2) {
      pages.unshift(-1);
    }
    if (current + delta < total - 1) {
      pages.push(-1);
    }

    pages.unshift(1);
    if (total > 1) {
      pages.push(total);
    }

    return pages;
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  get startItemIndex(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endItemIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.filteredContrats.length);
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR');
  }

  resetFilters() {
    this.searchTerm = '';
    this.filterPrestataire = '';
    this.filterTypeContrat = '';
    this.filterStatut = '';
    this.sortColumn = '';
    this.applyFilters();
  }

  //  Méthode pour afficher les initiales
  getInitials(nom?: string, prenom?: string): string {
    const n = nom?.charAt(0).toUpperCase() || '';
    const p = prenom?.charAt(0).toUpperCase() || '';
    return n + p || '??';
  }

  //  URL image avatar prestataire
  getAvatarUrl(imageUrl?: string): string | null {
    if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  //  AJOUT : Méthodes permissions
  getCurrentUserPrestataireId(): string | null {
    const currentUser = this.permissionService['authService'].currentUserValue;
    if (!currentUser) return null;
    
    /* // Chercher dans prestataires
    const prestataire = this.prestataires.find(p => p.userId === currentUser.id); */
    return currentUser?.prestataireId || null;
  }

  canManageContrat(): boolean {
    return this.permissionService.hasPermission(Permission.MANAGE_CONTRATS);
  }

  canViewTauxJournalier(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_TAUX_JOURNALIER);
  }

  // ========================================================================
  //  GESTION UPLOAD DOCUMENT
  // ========================================================================

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const maxSize = 10 * 1024 * 1024; // 10 MB
      const allowedTypes = ['application/pdf', 'image/jpeg', 'image/png',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];

      if (file.size > maxSize) {
        this.showNotification('warning', 'Le fichier ne doit pas dépasser 10 Mo.');
        return;
      }
      if (!allowedTypes.includes(file.type)) {
        this.showNotification('warning', 'Format non autorisé. Utilisez PDF, Word ou image.');
        return;
      }
      this.selectedFile = file;
    }
  }

  uploadDocument(contratId: string, file: File, onSuccess: () => void): void {
    this.isUploading = true;

    this.contratService.uploadDocument(contratId, file).subscribe({
      next: () => {
        this.isUploading = false;
        onSuccess();
      },
      error: (err) => {
        this.isUploading = false;
        this.showNotification('warning', 'Contrat sauvegardé, mais une erreur est survenue lors du téléchargement du document.');
        onSuccess(); // On continue quand même
      }
    });
  }

  ouvrirDocument(contrat: any): void {
    if (contrat.documentUrl) {
      const url = contrat.documentUrl.startsWith('http')
        ? contrat.documentUrl
        : `${environment.apiUrl.replace('/api', '')}/${contrat.documentUrl}`;
      window.open(url, '_blank');
    }
  }

  // ========================================================================
  //  NOUVEAU : STATUT COULEUR + TOGGLE ACTIF
  // ========================================================================

  // Retourne 'danger' si dépassé ou date expirée, 'warning' si ≤ 20j restants, 'success' sinon, 'inactive' si désactivé
  getContratAlertStatus(contrat: ContratDto): 'danger' | 'warning' | 'success' | 'inactive' {
    if (!(contrat.active ?? true)) return 'inactive';
    if (contrat.dateFin) {
      const fin = new Date(contrat.dateFin);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (fin < today) return 'danger';
    }
    const restants = this.getJoursRestants(contrat);
    if (restants < 0) return 'danger';
    if (restants <= 20) return 'warning';
    return 'success';
  }

  toggleContratActive(contrat: ContratDto): void {
    const newActive = !(contrat.active ?? true);
    this.contratService.update(contrat.id!, { ...contrat, active: newActive }).subscribe({
      next: () => {
        contrat.active = newActive;
        this.showNotification('success', `Contrat ${newActive ? 'activé' : 'désactivé'} avec succès`);
        this.cdr.detectChanges();
      },
      error: () => this.showNotification('error', 'Erreur lors de la mise à jour')
    });
  }

  isDateExpired(dateStr: string): boolean {
    const d = new Date(dateStr);
    d.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return d < today;
  }
  
}