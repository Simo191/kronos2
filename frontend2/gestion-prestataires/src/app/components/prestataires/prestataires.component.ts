import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PrestataireService } from '../../services/prestataire.service';
import { ContratDto, ContratService } from '../../services/contrat.service';
import { Prestataire } from '../../models/prestataire.model';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';
// ✅ AJOUT PERMISSIONS
import { PermissionService, Permission, Role } from '../../services/permission.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';
import { HasPermissionDirective } from '../../directives/permission.directives';
import { environment } from '../../../environments/environment';
//   Comptes prestataire (ADM/ADML/VPN…)
import { AuthService } from '../../services/auth.service';
import { PrestataireCredentialService } from '../../services/prestataire-credential.service';
import { PrestataireCredential } from '../../models/prestataire-credential.model';
import { EntrepriseService } from '../../services/entreprise.service';
import { Entreprise } from '../../models/entreprise.model';

@Component({
  selector: 'app-prestataires',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FormatMontantPipe,
    HasPermissionDirective // ✅ AJOUT
    ,
    NotificationToastComponent
],
  templateUrl: './prestataires.component.html',
  styleUrls: ['./prestataires.component.scss']
})
export class PrestatairesComponent implements OnInit {
  prestataires: Prestataire[] = [];
  filteredPrestataires: Prestataire[] = [];
  selectedPrestataire: Partial<Prestataire> = {};
  
  // Contrats par prestataire
  contratsByPrestataire: { [key: string]: ContratDto[] } = {};
  
  // ✅ NOUVEAU : Compteur de shifts par contrat
  shiftsByContrat: { [key: string]: number } = {};
 
  // Modal détails contrat
  showContratModal = false;
  selectedContrat: ContratDto | null = null;
  
  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  viewMode: 'list' | 'cards' = 'cards';

  // ✅ Expand/Collapse groupes
  internesCollapsed = false;
  prestatairesCollapsed = false;

  toggleInternes(): void {
    this.internesCollapsed = !this.internesCollapsed;
  }

  togglePrestataires(): void {
    this.prestatairesCollapsed = !this.prestatairesCollapsed;
  }

  expandAll(): void {
    this.internesCollapsed = false;
    this.prestatairesCollapsed = false;
  }

  collapseAll(): void {
    this.internesCollapsed = true;
    this.prestatairesCollapsed = true;
  }

  get allCollapsed(): boolean {
    return this.internesCollapsed && this.prestatairesCollapsed;
  }
  
  // Filtres
  searchTerm = '';
  filterTJM = '';
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  filterPrestataire = '';
  //   '' = tous | 'actif' | 'inactif'
  filterStatut: '' | 'actif' | 'inactif' = '';

  // ── COMPTES (ADM/ADML/VPN…) ─────────────────────────────────────────────
  showCredentialsModal = false;
  credentialsPrestataire: Prestataire | null = null;
  credentials: PrestataireCredential[] = [];
  credentialsLoading = false;
  //   Suggestions d'auto-complétion pour le label (libre mais on aide)
  readonly LABEL_SUGGESTIONS: string[] = ['ADM', 'ADML', 'VPN', 'Bastion', 'SSH', 'BDD', 'Autre'];
  //   Formulaire d'édition (création OU modification d'un compte)
  credForm: {
    id: string | null;
    label: string;
    accountName: string;
    password: string;       // saisi seulement lors d'une création OU d'un reset (mode édition : vide = on garde)
    expiresAt: string;
    note: string;
  } = { id: null, label: '', accountName: '', password: '', expiresAt: '', note: '' };
  isCredEditing = false;        // true = formulaire ouvert
  credIsEditMode = false;       // true = on modifie un cred existant
  showDeleteCredConfirm = false;
  credToDelete: PrestataireCredential | null = null;

  // ── REVEAL MOT DE PASSE ─────────────────────────────────────────────────
  showRevealModal = false;
  revealingCredential: PrestataireCredential | null = null;
  revealUserPassword = '';
  revealedPassword: string | null = null;
  revealError = '';
  revealLoading = false;
  private revealAutoHideTimer: any = null;
  readonly REVEAL_AUTO_HIDE_MS = 30000; // masquage auto au bout de 30 s

  //   Confirmation toggle activation
  showToggleActiveConfirm = false;
  prestataireToToggle: Prestataire | null = null;

  //   Motif de désactivation : liste prédéfinie + champ libre quand "Autre"
  //   - toggleReasonOption : valeur sélectionnée dans le select ('' = non renseigné)
  //   - toggleReasonCustom : précision libre, utilisée uniquement quand "Autre"
  readonly deactivationReasonOptions: string[] = [
    'Fin de mission',
    'Départ',
    'Congé longue durée',
    'Autre'
  ];
  toggleReasonOption: string = '';
  toggleReasonCustom: string = '';
  // Pagination
  currentPage = 1;
  pageSize = 10;
  
  // Statistiques
  totalPrestataires = 0;
  tjmMoyen = 0;
  tjmMax = 0;
  tjmMin = 0;
  
  // Notification
  
  // Confirmation suppression
  showDeleteConfirm = false;
  
  // ✅ HISTORIQUE CONTRATS
  showHistoriqueModal = false;
  historiquePrestataire: Prestataire | null = null;
  filtreHistorique: 'tous' | 'actifs' | 'expires' = 'tous';
  prestataireToDelete: Prestataire | null = null;

  // ✅ NOUVEAU : Upload image avatar
  selectedImageFile: File | null = null;
  imagePreview: string | null = null;

  // ✅ AJOUT PERMISSIONS
  Permission = Permission;
  Role = Role;

  //   Liste des entreprises disponibles (chargée au démarrage, alimente le select du modal)
  entreprises: Entreprise[] = [];

  constructor(
    private prestataireService: PrestataireService,
    private contratService: ContratService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private permissionService: PermissionService,  // ✅ AJOUT
    private notifService: NotificationService,
    private credService: PrestataireCredentialService,
    private authService: AuthService,
    private entrepriseService: EntrepriseService
  ) {}

  ngOnInit(): void {
    this.loadPrestataires();
    //   Charge la liste d'entreprises (utilisée par le select du modal)
    //   silencieux : si l'utilisateur n'y a pas accès, on garde liste vide.
    this.entrepriseService.getAll(false).subscribe({
      next: (list) => { this.entreprises = list ?? []; this.cdr.detectChanges(); },
      error: () => { this.entreprises = []; }
    });
  }

  //   Helper : libellé "Nom Prénom (CODE)" — utilisé partout (cartes, shift planning, listes)
  formatPrestataireWithEntreprise(p: Prestataire | null | undefined): string {
    if (!p) return '';
    const base = `${p.prenom ?? ''} ${p.nom ?? ''}`.trim();
    const tag = p.entreprise?.code || p.entreprise?.nom;
    return tag ? `${base} (${tag})` : base;
  }

  loadPrestataires(): void {
    this.isLoading = true;
    //   Inclut les comptes désactivés (affichés grisés). Les autres modules
    //   utilisent getAll() qui ne renvoie que les actifs.
    this.prestataireService.getAllIncludingInactive().subscribe({
      next: (data) => {

        this.prestataires = data ?? [];
        this.filteredPrestataires = data ;//[...this.prestataires];
        this.calculateStats();
        this.loadAllContrats(); // ✅ NOUVEAU : Charger les contrats
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du chargement des collaborateurs');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ MODIFIÉ : Calcul stats TJM selon permission
  calculateStats() {
    this.totalPrestataires = this.filteredPrestataires.filter(f=>f.responsable !== true).length;
    
    // ✅ Calculer TJM seulement si permission
    if (this.canViewTJM()) {
      if (this.filteredPrestataires.length > 0) {
        const totalTjm = this.filteredPrestataires.filter(f=>f.responsable !== true).reduce((sum, p) => sum + (p.tjm || 0), 0);
        this.tjmMoyen = Math.round(totalTjm / this.filteredPrestataires.filter(f=>f.responsable !== true).length);
        this.tjmMax = Math.max(...this.filteredPrestataires.filter(f=>f.responsable !== true).map(p => p.tjm || 0));
        this.tjmMin = Math.min(...this.filteredPrestataires.filter(f=>f.responsable !== true).map(p => p.tjm || 0));
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

  // Application des filtres
  applyFilters() {
    this.filteredPrestataires = this.prestataires.filter(prestataire => {
      // Filtre par recherche
      const searchMatch = !this.searchTerm || 
        prestataire.nom?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        prestataire.prenom?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        prestataire.email?.toLowerCase().includes(this.searchTerm.toLowerCase());
 
        const prestataireMatch = !this.filterPrestataire
        ||
        prestataire.id === this.filterPrestataire;
      // Filtre par TJM
      let tjmMatch = true;
      if (this.filterTJM) {
        const tjm = prestataire.tjm || 0;
        if (this.filterTJM === '0-500') {
          tjmMatch = tjm >= 0 && tjm <= 500;
        } else if (this.filterTJM === '500-700') {
          tjmMatch = tjm > 500 && tjm <= 700;
        } else if (this.filterTJM === '700+') {
          tjmMatch = tjm > 700;
        }
      }

      //   Filtre activation
      let statutMatch = true;
      if (this.filterStatut === 'actif') statutMatch = prestataire.isActive !== false;
      else if (this.filterStatut === 'inactif') statutMatch = prestataire.isActive === false;

      return searchMatch && prestataireMatch && tjmMatch && statutMatch;
    });

    this.applySorting();
    this.calculateStats(); // ✅ NOUVEAU : Recalculer les stats après filtrage
    this.currentPage = 1;
  }
  // Modal
  openModal(prestataire?: Prestataire) {
    this.isEditMode = !!prestataire;
    this.selectedPrestataire = prestataire ? { ...prestataire } : {};
    this.selectedImageFile = null;
    // Si edition avec image existante, prévisualiser
    this.imagePreview = prestataire?.imageUrl
      ? this.getAvatarUrl(prestataire.imageUrl)
      : null;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedPrestataire = {};
    this.selectedImageFile = null;
    this.imagePreview = null;
  }

  savePrestataire() {
    // Validation
 

    if (!this.selectedPrestataire.nom || !this.selectedPrestataire.prenom || 
        !this.selectedPrestataire.email || !this.selectedPrestataire.tjm) {
      this.showNotification('warning', 'Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (this.isEditMode && this.selectedPrestataire.id) {
       this.prestataireService.update(this.selectedPrestataire.id, this.selectedPrestataire).subscribe({
        next: () => {
          this.uploadImageIfSelected(this.selectedPrestataire.id!);
          this.loadPrestataires();
          this.closeModal();
          this.showNotification('success', 'Collaborateur modifié avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la modification');
        }
      });
    } else {
      this.prestataireService.create(this.selectedPrestataire).subscribe({
        next: (created: any) => {
          const id = created?.id || created;
          if (id) this.uploadImageIfSelected(id);
          this.loadPrestataires();
          this.closeModal();
          this.showNotification('success', 'Collaborateur créé avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la création');
        }
      });
    }
  }

  // Confirmation de suppression
  confirmDelete(prestataire: Prestataire) {
    this.prestataireToDelete = prestataire;
    this.showDeleteConfirm = true;
  }

  deletePrestataire() {
    if (this.prestataireToDelete && this.prestataireToDelete.id) {
      this.prestataireService.delete(this.prestataireToDelete.id).subscribe({
        next: () => {
          this.loadPrestataires();
          this.showDeleteConfirm = false;
          this.prestataireToDelete = null;
          this.showNotification('success', 'Collaborateur supprimé avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la suppression');
          this.showDeleteConfirm = false;
          this.prestataireToDelete = null;
        }
      });
    }
  }

  // Notification
  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notifService.show(type, message);
  }

  // Pagination
  get totalPages(): number {
    return Math.ceil(this.filteredPrestataires.length / this.pageSize);
  }

  get pagedPrestataires(): Prestataire[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredPrestataires.slice(start, start + this.pageSize);
  }

  // ✅ GROUPEMENT : Internes (responsable=true) puis Prestataires (responsable=false)
  get groupedInterns(): Prestataire[] {
    return this.pagedPrestataires.filter(p => p.responsable === true);
  }

  get groupedPrestataires(): Prestataire[] {
    return this.pagedPrestataires.filter(p => p.responsable !== true);
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  get startItemIndex(): number {
    return this.filteredPrestataires.length === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get endItemIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.filteredPrestataires.length);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  // ========== CONTRATS ==========
  
  loadAllContrats(): void {
    this.contratService.getAll().subscribe({
      next: (contrats) => {
        // Grouper les contrats par prestataire
        this.contratsByPrestataire = {};
        contrats.forEach(contrat => {
          if (contrat.prestataire?.id) {
            const prestataireId = contrat.prestataire.id;
            if (!this.contratsByPrestataire[prestataireId]) {
              this.contratsByPrestataire[prestataireId] = [];
            }
            this.contratsByPrestataire[prestataireId].push(contrat);
          }
        });
        
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erreur chargement contrats:', err);
      }
    });
  }

  getContratsForPrestataire(prestataireId?: string): ContratDto[] {
    if (!prestataireId) return [];
    return this.contratsByPrestataire[prestataireId] || [];
  }

  getActiveContratsCount(prestataireId?: string): number {
    const contrats = this.getContratsForPrestataire(prestataireId);
    return contrats.filter(c => this.isContratActif(c)).length;
  }

  isContratActif(contrat: ContratDto): boolean {
    if (!contrat.dateFin) return true;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const dateFin = new Date(contrat.dateFin);
    dateFin.setHours(0, 0, 0, 0);
    return dateFin >= today;
  }

  viewContrat(contrat: ContratDto): void {
    this.selectedContrat = contrat;
    this.showContratModal = true;
  }

  closeContratModal(): void {
    this.showContratModal = false;
    this.selectedContrat = null;
  }

  getJoursRestants(contrat: ContratDto): number {
    const joursContrat = contrat.nombreJours || 0;
    const joursUtilises = contrat.joursUtilises || 0;
    const joursRestants = joursContrat - joursUtilises;
    return joursRestants > 0 ? joursRestants : 0;
  }

  formatDate(dateStr: any): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return isNaN(d.getTime()) ? '' : d.toLocaleDateString('fr-FR');
  }

  // Utilitaires
  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  getInitials(nom?: string, prenom?: string): string {
    const n = nom?.charAt(0).toUpperCase() || '';
    const p = prenom?.charAt(0).toUpperCase() || '';
    return n + p || '??';
  }

  // ✅ AJOUT : Méthode permission
  canViewTJM(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_TJM);
  }

  //   Admin/Manager peuvent créer/modifier/désactiver/supprimer un prestataire.
  //   User : non — il ne peut que consulter sa fiche et gérer ses comptes.
  canManagePrestataires(): boolean {
    return this.permissionService.hasPermission(Permission.MANAGE_PRESTATAIRES);
  }
   // Application du tri
// ✅ REMPLACER votre méthode applySorting() (ligne 380) par celle-ci :

applySorting() {
  if (!this.sortBy) return;
  
  this.filteredPrestataires.sort((a, b) => {
    let comparison = 0;
    
    if (this.sortBy === 'nom') {
      comparison = (a.nom || '').localeCompare(b.nom || '');
    } else if (this.sortBy === 'tjm-asc' || this.sortBy === 'tjm-desc') {
      comparison = (a.tjm || 0) - (b.tjm || 0);
    }
    
    // Appliquer la direction
    return this.sortDirection === 'asc' ? comparison : -comparison;
  });
}

  toggleSortDirection() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.applyFilters();
  }

  // ========================================================================
  // ✅ HISTORIQUE CONTRATS
  // ========================================================================

  // ========================================================================
  //   COMPTES TECHNIQUES (ADM / ADML / VPN…)
  // ========================================================================
  openCredentialsModal(p: Prestataire): void {
    if (!p?.id) return;
    this.credentialsPrestataire = p;
    this.credentials = [];
    this.isCredEditing = false;
    this.credIsEditMode = false;
    this.resetCredForm();
    this.showCredentialsModal = true;
    this.loadCredentials();
  }

  closeCredentialsModal(): void {
    this.showCredentialsModal = false;
    this.credentialsPrestataire = null;
    this.credentials = [];
    this.isCredEditing = false;
    this.resetCredForm();
    //   Si un reveal était en cours, nettoyer
    this.closeRevealModal();
  }

  private loadCredentials(): void {
    if (!this.credentialsPrestataire?.id) return;
    this.credentialsLoading = true;
    this.credService.getAll(this.credentialsPrestataire.id).subscribe({
      next: (list) => {
        this.credentials = list ?? [];
        this.credentialsLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.credentialsLoading = false;
        const msg = err?.status === 403
          ? 'Vous n\'avez pas accès aux comptes de ce collaborateur.'
          : 'Erreur lors du chargement des comptes.';
        this.showNotification('error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  private resetCredForm(): void {
    this.credForm = { id: null, label: '', accountName: '', password: '', expiresAt: '', note: '' };
  }

  startNewCred(): void {
    this.resetCredForm();
    this.credIsEditMode = false;
    this.isCredEditing = true;
  }

  startEditCred(c: PrestataireCredential): void {
    this.credForm = {
      id: c.id,
      label: c.label,
      accountName: c.accountName,
      password: '', //   vide = on ne change pas le mdp côté backend
      expiresAt: c.expiresAt ? c.expiresAt.substring(0, 10) : '',
      note: c.note ?? ''
    };
    this.credIsEditMode = true;
    this.isCredEditing = true;
  }

  cancelCredEdit(): void {
    this.isCredEditing = false;
    this.resetCredForm();
  }

  saveCred(): void {
    if (!this.credentialsPrestataire?.id) return;

    const label = this.credForm.label?.trim();
    const accountName = this.credForm.accountName?.trim();
    if (!label || !accountName) {
      this.showNotification('warning', 'Le type et l\'identifiant sont obligatoires.');
      return;
    }
    if (!this.credIsEditMode && !this.credForm.password) {
      this.showNotification('warning', 'Le mot de passe est obligatoire à la création.');
      return;
    }

    const expiresAt = this.credForm.expiresAt
      ? new Date(this.credForm.expiresAt).toISOString()
      : null;
    const note = this.credForm.note?.trim() || null;

    const prestataireId = this.credentialsPrestataire.id;

    if (this.credIsEditMode && this.credForm.id) {
      //   password vide = on garde l'ancien
      this.credService.update(prestataireId, this.credForm.id, {
        label, accountName,
        password: this.credForm.password || null,
        expiresAt, note
      }).subscribe({
        next: () => {
          this.showNotification('success', 'Compte modifié.');
          this.isCredEditing = false;
          this.resetCredForm();
          this.loadCredentials();
        },
        error: () => this.showNotification('error', 'Erreur lors de la modification.')
      });
    } else {
      this.credService.create(prestataireId, {
        label, accountName,
        password: this.credForm.password,
        expiresAt, note
      }).subscribe({
        next: () => {
          this.showNotification('success', 'Compte ajouté.');
          this.isCredEditing = false;
          this.resetCredForm();
          this.loadCredentials();
        },
        error: () => this.showNotification('error', 'Erreur lors de la création.')
      });
    }
  }

  confirmDeleteCred(c: PrestataireCredential): void {
    this.credToDelete = c;
    this.showDeleteCredConfirm = true;
  }
  cancelDeleteCred(): void {
    this.showDeleteCredConfirm = false;
    this.credToDelete = null;
  }
  doDeleteCred(): void {
    if (!this.credentialsPrestataire?.id || !this.credToDelete?.id) return;
    this.credService.delete(this.credentialsPrestataire.id, this.credToDelete.id).subscribe({
      next: () => {
        this.showNotification('success', 'Compte supprimé.');
        this.showDeleteCredConfirm = false;
        this.credToDelete = null;
        this.loadCredentials();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors de la suppression.');
        this.showDeleteCredConfirm = false;
        this.credToDelete = null;
      }
    });
  }

  // ── Reveal ───────────────────────────────────────────────────────────────
  askReveal(c: PrestataireCredential): void {
    this.revealingCredential = c;
    this.revealUserPassword = '';
    this.revealedPassword = null;
    this.revealError = '';
    this.revealLoading = false;
    this.showRevealModal = true;
  }

  closeRevealModal(): void {
    if (this.revealAutoHideTimer) {
      clearTimeout(this.revealAutoHideTimer);
      this.revealAutoHideTimer = null;
    }
    this.showRevealModal = false;
    this.revealingCredential = null;
    this.revealUserPassword = '';
    this.revealedPassword = null;
    this.revealError = '';
    this.revealLoading = false;
  }

  confirmReveal(): void {
    if (!this.credentialsPrestataire?.id || !this.revealingCredential?.id) return;
    if (!this.revealUserPassword) {
      this.revealError = 'Veuillez saisir votre mot de passe.';
      return;
    }
    this.revealLoading = true;
    this.revealError = '';
    this.credService.reveal(this.credentialsPrestataire.id, this.revealingCredential.id, this.revealUserPassword)
      .subscribe({
        next: (res) => {
          this.revealedPassword = res.password;
          this.revealUserPassword = ''; //   on efface la saisie immédiatement
          this.revealLoading = false;
          //   Masquage automatique au bout de N secondes
          if (this.revealAutoHideTimer) clearTimeout(this.revealAutoHideTimer);
          this.revealAutoHideTimer = setTimeout(() => {
            this.revealedPassword = null;
            this.showRevealModal = false;
            this.cdr.detectChanges();
          }, this.REVEAL_AUTO_HIDE_MS);
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.revealLoading = false;
          this.revealError = err?.status === 401
            ? 'Mot de passe incorrect.'
            : 'Impossible de récupérer le mot de passe.';
          this.cdr.detectChanges();
        }
      });
  }

  copyRevealedPassword(): void {
    if (!this.revealedPassword) return;
    navigator.clipboard.writeText(this.revealedPassword).then(
      () => this.showNotification('success', 'Mot de passe copié dans le presse-papiers.'),
      () => this.showNotification('error', 'Impossible de copier (autoriser le presse-papiers ?).')
    );
  }

  //   Helpers d'affichage
  isCredExpired(c: PrestataireCredential): boolean {
    if (!c.expiresAt) return false;
    return new Date(c.expiresAt).getTime() < Date.now();
  }
  isCredExpiringSoon(c: PrestataireCredential): boolean {
    if (!c.expiresAt) return false;
    const ms = new Date(c.expiresAt).getTime() - Date.now();
    return ms > 0 && ms < 14 * 24 * 3600 * 1000; // moins de 14 jours
  }

  /**
   *   Le bouton "Comptes" est-il affichable sur cette carte pour l'utilisateur courant ?
   *   - Admin/Manager : oui pour tout le monde
   *   - User : uniquement sur sa propre carte (prestataire lié au userId du JWT)
   */
  canSeeCredentials(p: Prestataire): boolean {
    const user = this.authService.currentUserValue;
    if (!user) return false;
    const role = user.role?.name;
    if (role === 'Admin' || role === 'Manager') return true;
    //   role User : doit être le propriétaire
    return !!p.id && !!user.prestataireId && p.id === user.prestataireId;
  }

  // ========================================================================
  //   TOGGLE ACTIVATION (désactiver = masqué partout sauf liste + connexion bloquée)
  // ========================================================================
  confirmToggleActive(prestataire: Prestataire): void {
    this.prestataireToToggle = prestataire;
    //   Vider les saisies à chaque ouverture (raison uniquement utile pour désactivation)
    this.toggleReasonOption = '';
    this.toggleReasonCustom = '';
    this.showToggleActiveConfirm = true;
  }

  cancelToggleActive(): void {
    this.showToggleActiveConfirm = false;
    this.prestataireToToggle = null;
    this.toggleReasonOption = '';
    this.toggleReasonCustom = '';
  }

  //   Motif final envoyé au backend (null si rien de cohérent)
  private buildDeactivationReason(): string | null {
    if (!this.toggleReasonOption) return null;
    if (this.toggleReasonOption === 'Autre') {
      const custom = this.toggleReasonCustom?.trim();
      return custom ? custom : null;
    }
    return this.toggleReasonOption;
  }

  toggleActive(): void {
    const target = this.prestataireToToggle;
    if (!target?.id) return;

    //   Raison envoyée uniquement quand on désactive (target actif = on va le désactiver)
    const willDeactivate = target.isActive !== false;
    const reason = willDeactivate ? this.buildDeactivationReason() : null;

    this.prestataireService.toggleActive(target.id, reason).subscribe({
      next: (res) => {
        //   Mise à jour locale immédiate (état + traçabilité)
        const apply = (p: Prestataire) => {
          p.isActive = res.isActive;
          if (!res.isActive) {
            p.deactivationReason = reason;
            p.deactivatedAt = new Date().toISOString();
          } else {
            p.deactivationReason = null;
            p.deactivatedAt = null;
          }
        };
        const inList = this.prestataires.find(p => p.id === target.id);
        if (inList) apply(inList);
        if (target) apply(target);

        this.showNotification(
          'success',
          res.isActive
            ? `${target.prenom} ${target.nom} a été réactivé`
            : `${target.prenom} ${target.nom} a été désactivé`
        );
        this.applyFilters();
        this.cancelToggleActive();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du changement d\'état');
        this.cancelToggleActive();
      }
    });
  }

  //   Tooltip lisible sur le badge "Désactivé"
  getDeactivationTooltip(p: Prestataire): string {
    const parts: string[] = [];
    if (p.deactivatedAt) {
      const d = new Date(p.deactivatedAt);
      if (!isNaN(d.getTime())) {
        parts.push(`Désactivé le ${d.toLocaleDateString('fr-FR')} à ${d.toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' })}`);
      }
    }
    if (p.deactivationReason) {
      parts.push(`Motif : ${p.deactivationReason}`);
    }
    return parts.length > 0 ? parts.join('\n') : 'Compte désactivé';
  }

  // ========================================================================
  // ✅ NOUVEAU : TOGGLE RESPONSABLE
  // ========================================================================
  toggleResponsable(prestataire: Prestataire): void {
    const updated = { ...prestataire, responsable: !prestataire.responsable };
    this.prestataireService.update(prestataire.id!, updated).subscribe({
      next: () => {
        prestataire.responsable = !prestataire.responsable;
        this.showNotification('success', `${prestataire.prenom} ${prestataire.nom} est maintenant ${prestataire.responsable ? 'Interne' : 'Prestataire'}`);
        this.cdr.detectChanges();
      },
      error: () => this.showNotification('error', 'Erreur lors de la mise à jour du type')
    });
  }

  // ========================================================================
  // ✅ NOUVEAU : IMAGE AVATAR
  // ========================================================================

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    if (file.size > 5 * 1024 * 1024) {
      this.showNotification('warning', 'Image trop lourde (max 5 Mo)');
      return;
    }
    this.selectedImageFile = file;
    // Prévisualisation locale
    const reader = new FileReader();
    reader.onload = (e) => {
      this.imagePreview = e.target?.result as string;
      this.cdr.detectChanges();
    };
    reader.readAsDataURL(file);
  }

  removeImage(): void {
    this.selectedImageFile = null;
    this.imagePreview = null;
    this.selectedPrestataire.imageUrl = undefined;
  }

  // Construire l'URL complète de l'avatar depuis environment
  getAvatarUrl(imageUrl?: string): string | null {
    if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  // Après create/update, uploader l'image si sélectionnée
  private uploadImageIfSelected(prestataireId: string): void {
    if (!this.selectedImageFile) return;
    this.prestataireService.uploadImage(prestataireId, this.selectedImageFile).subscribe({
      next: () => {
        this.loadPrestataires();
        this.selectedImageFile = null;
        this.imagePreview = null;
      },
      error: () => this.showNotification('warning', 'Collaborateur sauvegardé mais erreur upload image')
    });
  }

  viewHistorique(prestataire: Prestataire): void {
    if (!prestataire?.id) return;
  
    // Ouvre la page Contrats en filtrant sur ce prestataire
    this.router.navigate(['/contrats'], {
      queryParams: { prestataireId: prestataire.id }
    });
  }
  

  closeHistoriqueModal(): void {
    this.showHistoriqueModal = false;
    this.historiquePrestataire = null;
    this.filtreHistorique = 'tous';
    this.cdr.detectChanges(); // ✅ FORCER la détection
  }

  getContratsHistorique(): ContratDto[] {
    if (!this.historiquePrestataire?.id) return [];
    const contrats = this.getContratsForPrestataire(this.historiquePrestataire.id);
    if (this.filtreHistorique === 'actifs') {
      return contrats.filter(c => this.isContratActif(c));
    } else if (this.filtreHistorique === 'expires') {
      return contrats.filter(c => !this.isContratActif(c));
    }
    return contrats;
  }

  getTotalContratsCount(prestataireId?: string): number {
    return this.getContratsForPrestataire(prestataireId).length;
  }

  getExpiredContratsCount(prestataireId?: string): number {
    return this.getContratsForPrestataire(prestataireId).filter(c => !this.isContratActif(c)).length;
  }

  getTotalJoursAllContrats(prestataireId?: string): number {
    return this.getContratsForPrestataire(prestataireId).reduce((sum, c) => sum + (c.nombreJours || 0), 0);
  }

  getContratStatut(contrat: ContratDto): string {
    return this.isContratActif(contrat) ? 'Actif' : 'Expiré';
  }

  getContratStatutClass(contrat: ContratDto): string {
    return this.isContratActif(contrat) ? 'badge-success' : 'badge-secondary';
  }

  getContratProgress(contrat: ContratDto): number {
    const total = contrat.nombreJours || 0;
    const utilises = contrat.joursUtilises || 0;
    if (total === 0) return 0;
    return Math.min(Math.round((utilises / total) * 100), 100);
  }
}