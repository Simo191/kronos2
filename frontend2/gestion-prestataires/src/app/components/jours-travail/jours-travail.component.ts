import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { JourTravailService, JourTravailDto } from '../../services/jour-travail.service';
import { PrestataireService } from '../../services/prestataire.service';
import { ContratService, ContratDto } from '../../services/contrat.service';
import { CagnotteService } from '../../services/cagnotte.service';

import { Prestataire } from '../../models/prestataire.model';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';

 

// ✅ AJOUT PERMISSIONS - 5 imports
import { Permission, PermissionService, Role } from '../../services/permission.service';
import { HasPermissionDirective, HasRoleDirective } from '../../directives/permission.directives';
import { MoisVerrouilleService } from '../../services/verrouille.service';
import { environment } from '../../../environments/environment';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';
  

@Component({
  selector: 'app-jours-travail',
  standalone: true,
  imports: [CommonModule, FormsModule, FormatMontantPipe, NotificationToastComponent], // ✅ AJOUT 2 directives
  templateUrl: './jours-travail.component.html',
  styleUrls: ['./jours-travail.component.scss']
})
export class JoursTravailComponent implements OnInit {
  // ✅ AJOUT PERMISSIONS - Exposer au template
  Permission = Permission;
  Role = Role;

  joursTravail: JourTravailDto[] = [];
  filteredJours: JourTravailDto[] = [];
  prestataires: Prestataire[] = [];
  //   Toggle "Inclure désactivés" partagé entre filtre et sélecteur
  showInactivePrest = false;

  // Contrats (pour récupérer TJM + nombreJours)
  contrats: ContratDto[] = [];
  isLoadingContrats = false;

  selectedJour: Partial<JourTravailDto> = {};
  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  viewMode: 'list' | 'cards' = 'cards';

  // Filtres
  searchTerm = '';
  filterPrestataire = '';
  filterMois = '';// `${new Date().getFullYear()}-${String(new Date().getMonth() + 1).padStart(2, '0')}`;
  filterAnnee = `${new Date().getFullYear()}`; // ✅ NOUVEAU FILTRE

  // ✅ TRI
  sortColumn: 'prestataire' | 'mois' | 'jours' | 'tjm' | 'revenu' | '' = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Statistiques (calculées sur filteredJours)
  totalJours = 0;
  revenuTotal = 0;
  tjmMoyen = 0;

  // Liste des mois et années uniques pour les filtres
  moisUniques: string[] = [];
  anneesUniques: string[] = []; // ✅ NOUVEAU

  // Notification

  // État TJM trouvé ou non
  tjmFromContratFound = true;
  tjmSourceContratId?: string;
  openDropdownId: string | null = null;

  // Info sur le quota du contrat
  contratQuotaInfo = {
    nombreJoursContrat: 0,
    joursDejaUtilises: 0,
    joursRestants: 0,
    pourcentageUtilise: 0
  };

  // Confirmation suppression
  showDeleteConfirm = false;
  jourToDelete: JourTravailDto | null = null;

  // ✅ Menu 3 points cartes
  activeCardMenuId: string | null = null;

  // Month picker personnalisé
  showMonthPicker = false;
  selectedYear = new Date().getFullYear();
  moisList = [
    { value: 1, label: 'Janvier' },
    { value: 2, label: 'Février' },
    { value: 3, label: 'Mars' },
    { value: 4, label: 'Avril' },
    { value: 5, label: 'Mai' },
    { value: 6, label: 'Juin' },
    { value: 7, label: 'Juillet' },
    { value: 8, label: 'Août' },
    { value: 9, label: 'Septembre' },
    { value: 10, label: 'Octobre' },
    { value: 11, label: 'Novembre' },
    { value: 12, label: 'Décembre' }
  ];

  constructor(
    private jourTravailService: JourTravailService,
    private prestataireService: PrestataireService,
    private contratService: ContratService,
    private cagnotteService: CagnotteService,
    private cdr: ChangeDetectorRef,
    private permissionService: PermissionService, // ✅ AJOUT permissionService
    private moisVerrouilleService: MoisVerrouilleService, // ✅ AJOUTER
    private notifService: NotificationService
  ) {}

  ngOnInit() {
    this.loadJoursTravail();
    this.loadPrestataires();
    this.loadContrats();
    this.moisVerrouilleService.loadMoisVerrouilles(); // ✅ AJOUTER

    document.addEventListener('click', () => {
      this.closeAllDropdowns();
      this.cdr.detectChanges();
    });
  }

  isMoisVerrouille(mois: string): boolean {
    return this.moisVerrouilleService.isMoisVerrouille(mois);
  }

  // ✅ AJOUT PERMISSIONS - 3 nouvelles méthodes
  canViewTJM(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_TJM);
  }

  canViewRevenu(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_REVENUE);
  }

  canManage(): boolean {
    return this.permissionService.hasPermission(Permission.MANAGE_JOURS_TRAVAIL);
  }
  canManageUpdate(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_JOURS_TRAVAIL);
  }
  // ✅ NOUVEAU : User peut modifier SES propres jours
  canModifyJour(jour: JourTravailDto): boolean {
    // Admin/Manager peuvent modifier tous les jours
    if (this.canManage()) {
      return true;
    }
    
    // User peut modifier seulement SES jours
    const userPrestataireId = this.getCurrentUserPrestataireId();
    return jour.prestataireId === userPrestataireId;
  }
// ✅ NOUVEAU : User peut modifier SES propres jours
canDeleteJour(jour: JourTravailDto): boolean {
  // Admin/Manager peuvent modifier tous les jours
   if (this.canManageUpdate()) {
    return true;
  }
  
  // User peut modifier seulement SES jours
  const userPrestataireId = this.getCurrentUserPrestataireId();
  return jour.prestataireId === userPrestataireId;
}
  // ✅ AJOUT : Récupérer le prestataireId du User connecté
  getCurrentUserPrestataireId(): string | null {
    const currentUserStr = localStorage.getItem('currentUser');
    if (currentUserStr) {
      try {
        const currentUser = JSON.parse(currentUserStr);
        return currentUser?.prestataire?.id || null;
      } catch {
        return null;
      }
    }
    return null;
  }

  loadJoursTravail() {
    this.isLoading = true;
    this.jourTravailService.getAll().subscribe({
      next: (data) => {
        this.joursTravail = data ?? [];
        this.filteredJours = [...this.joursTravail];
        this.extractMoisUniques();
        this.extractAnneesUniques(); // ✅ NOUVEAU
        
        // ✅ FIX : Ré-appliquer les filtres après rechargement
        if (this.searchTerm || this.filterPrestataire || this.filterMois || this.filterAnnee || this.sortColumn) {
          this.applyFilters();
        } else {
          this.calculateStats();
        }
        
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du chargement des données');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadPrestataires() {
    this.prestataireService.getAll(this.showInactivePrest).subscribe({
      next: (data) => {
        this.prestataires = (data ?? []).filter(p=>p.responsable !== true);
        this.cdr.detectChanges();
      },
      error: () => {
        console.error('Erreur chargement prestataires');
        this.cdr.detectChanges();
      }
    });
  }

  //   Bascule l'inclusion des prestataires désactivés (rechargement)
  toggleShowInactivePrest(): void {
    this.showInactivePrest = !this.showInactivePrest;
    //   Nettoie le filtre si un inactif est sélectionné
    if (!this.showInactivePrest && (this as any).filterPrestataire) {
      const sel = this.prestataires.find((p: any) => p.id === (this as any).filterPrestataire);
      if ((sel as any)?.isActive === false) (this as any).filterPrestataire = '';
    }
    this.loadPrestataires();
  }

  loadContrats() {
    this.isLoadingContrats = true;
    this.contratService.getAll().subscribe({
      next: (data) => {
        this.contrats = data ?? [];
        this.isLoadingContrats = false;

        if (this.isModalOpen) {
          this.updateTjmAndQuotaFromContrat();
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingContrats = false;
        this.showNotification('error', 'Erreur lors du chargement des contrats');
        this.cdr.detectChanges();
      }
    });
  }

  extractMoisUniques() {
    const moisSet = new Set(this.joursTravail.map(j => j.mois));
    this.moisUniques = Array.from(moisSet).sort().reverse();
  }

  // ✅ NOUVEAU : Extraire les années uniques
  extractAnneesUniques() {
    const anneesSet = new Set(
      this.joursTravail.map(j => {
        const [annee] = j.mois.split('-');
        return annee;
      })
    );
    this.anneesUniques = Array.from(anneesSet).sort().reverse();
  }

  // ✅ MODIFIÉ : Calcul sur filteredJours au lieu de joursTravail
  calculateStats() {
    this.totalJours = this.filteredJours.reduce((sum, j) => sum + (j.nombreJours || 0), 0);
    this.revenuTotal = this.filteredJours.reduce((sum, j) => sum + ((j.nombreJours || 0) * (j.tjm || 0)), 0);

    const totalTjm = this.filteredJours.reduce((sum, j) => sum + (j.tjm || 0), 0);
    this.tjmMoyen = this.filteredJours.length > 0
      ? Math.round(totalTjm / this.filteredJours.length)
      : 0;
  }

  // ✅ MODIFIÉ : Ajout filtre année
  applyFilters() {
    this.filteredJours = this.joursTravail.filter(jour => {
      const searchMatch = !this.searchTerm ||
        jour.prestataire?.nom?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        jour.prestataire?.prenom?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        this.formatMois(jour.mois).toLowerCase().includes(this.searchTerm.toLowerCase());

      const prestataireMatch = !this.filterPrestataire ||
        jour.prestataireId === this.filterPrestataire;

      const moisMatch = !this.filterMois ||
        jour.mois === this.filterMois;

      // ✅ NOUVEAU : Filtre par année
      const anneeMatch = !this.filterAnnee ||
        jour.mois.startsWith(this.filterAnnee);

      return searchMatch && prestataireMatch && moisMatch && anneeMatch;
    });

    // ✅ IMPORTANT : Appliquer le tri après filtrage
    this.applySorting();

    // ✅ IMPORTANT : Recalculer les stats après filtrage
    this.calculateStats();
    this.cdr.detectChanges();
  }

  // ✅ NOUVEAU : Méthode de tri
  sortBy(column: 'prestataire' | 'mois' | 'jours' | 'tjm' | 'revenu') {
    if (this.sortColumn === column) {
      // Inverser la direction si on clique sur la même colonne
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Nouvelle colonne, commencer par ascendant
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.applySorting();
    this.cdr.detectChanges();
  }

  // ✅ NOUVEAU : Gérer le changement du dropdown
  onSortChange() {
    if (!this.sortColumn) {
      // Pas de tri sélectionné
      this.sortDirection = 'asc';
      this.filteredJours = [...this.joursTravail];
      this.applyFilters();
    } else {
      // Appliquer le tri
      this.applySorting();
    }
    this.cdr.detectChanges();
  }

  // ✅ NOUVEAU : Basculer la direction du tri
  toggleSortDirection() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    if (this.sortColumn) {
      this.applySorting();
      this.cdr.detectChanges();
    }
  }

  // ✅ NOUVEAU : Appliquer le tri
  private applySorting() {
    if (!this.sortColumn) return;

    this.filteredJours.sort((a, b) => {
      let comparison = 0;

      switch (this.sortColumn) {
        case 'prestataire':
          const nameA = `${a.prestataire?.nom || ''} ${a.prestataire?.prenom || ''}`;
          const nameB = `${b.prestataire?.nom || ''} ${b.prestataire?.prenom || ''}`;
          comparison = nameA.localeCompare(nameB);
          break;

        case 'mois':
          comparison = (a.mois || '').localeCompare(b.mois || '');
          break;

        case 'jours':
          comparison = (a.nombreJours || 0) - (b.nombreJours || 0);
          break;

        case 'tjm':
          comparison = (a.tjm || 0) - (b.tjm || 0);
          break;

        case 'revenu':
          const revenuA = (a.nombreJours || 0) * (a.tjm || 0);
          const revenuB = (b.nombreJours || 0) * (b.tjm || 0);
          comparison = revenuA - revenuB;
          break;
      }

      return this.sortDirection === 'asc' ? comparison : -comparison;
    });
  }

  // ========== ✅ GROUPEMENT PAR MOIS/ANNÉE ==========

  // ✅ Couleurs & icônes zodiacales par mois (images HD individuelles)
  private readonly ZODIAC_BY_MONTH: {
    bg: string; bg2: string; light: string; border: string;
    img: string; name: string;
  }[] = [
    // Capricorne
    { bg: '#f5c518', bg2: '#c9960c', light: '#fef9e7', border: '#f9d857',
      img: 'data:image/png;base64,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', name: 'Capricorne' },
    // Verseau
    { bg: '#3b35e2', bg2: '#2825b8', light: '#eeeeff', border: '#6b66ee',
      img: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAARpUlEQVR42r1ae5RdZXX/7f195577mpk7j0zmlZAXBSGBJECISAjSVmsVFHWxqoiYgi6VCi2tFrGrsgpqBFlKNaJgbanLLmsFFFcp6DIKWl5JyYMKCgYSSCaZZB733pn7OPec79v945y5c+beOzPBFL41a9a9537nnL33t5+/vUlEMHtZC+bwo1QrwYH91ZdfDoaP2LGjgR8E5bIqT5EIgkAAgCi6bfqbACKgmWvRoukL4Qu1IuWEm4QIxJTNiuuaVMbp7lKL+3hoSWLJkJNKa4ABGAOl0LyogQFjoRiA+dUvSz95qLx3T3D0iC2XYWxEKBEINE25RISFRBMJBIg2NCyJ/gmBpH5L9AgREAQSiUG0QjIpvYv5lDfozRemLnxzJpNxAAnf0poBEYiAGU88Udx2R37X02Ktcl1KJMAc0S2ob54hvS54iABEVP9ViKLnE4XXY/xI7F5pZFqErBW/Bs+zVmTZcnzgysz7Lm9ncqwVZmpkQAQiwixf/+roN+8sA042SyI2fND0m0hkhqa5VsM2IihFQUDGCDO0JiIxpi6F6Jb419hFYQagPE9KpeD88/mWrT19/cmYkk8zYIwoJTffdPQ793g9PQ4gxsg8xDW8Ly7p+HVmGMOlUpDLIZWmmof8hFjhtjaeZqPFXa1eC60wkZelS+03vtV90rJM/RxIREKG7tw28uXbqr2LHROIbaKmrnl1QhtE2EwBM2o1zrYFV38k8+aLMt3dulQye/ZUH/xxafvPfECn0wiCGR7iD28pDsehYlGWLZd7vtuby7mRQZpAWGHnzuKfXzGRzTrW2rihxMmdZZGzxd/MDBHEcsINvvHtztNPawPM2KifbVOu6wB44rHCFz5XeHEft7cjCEJ/0IKB5hN2HBobk4vfqW69fbG1zAwmhjHmji8ViFScvnDFyZXYaiC9+VhY0eSkuWJL+vTT2p56ovjuSw6+6x1HLn7b4euvO7TzqeLG8zru+W7f2efYYgHaofB582hR/Sffl64ufvDH/i8fmWSGMSARefTR4seunsjlnCCwC1on5l2xPaSUue+B3lxOv/uS4f0vcXsHB4FUKuJo8/4rkp/69KJSKbh6y5Fn/5ezGQrMnEfa8HxmKU1h/Tn07Xv6AMWA/PiHJUDNDjuNtzV6X6L4h3ADM0LXyQzPk/5B1def2PeCd/gwOrsYEK0ll6N0xvmnu71rrznkuur2Ly/q7DQ1v/H580jKWkpnePfT5rlnK0TgsfHa7qdNOkWha5vnBOv+J/6CmZ8YlQqJRPxYg44OADw+EZjA1lUvCMRau6hXP/xfuOFvjgwMJj95Q3up5Cs1y9LiIotfD5/CLNUKPfqLKgB+Zo83Omrritgs7LiYmwUTbteai0W74Y02mxVrw5BLrisiGB+TwDDFlggCX3oXqQd+FHzrrtG3X5zbsFFNTs649pYyir/RCrTDu3d5gOVnn/WCgJhiSU0TlXEzbZATAKX42LHgQ1vcG27sFArEktYIAjs4mCDCM894AE9nQVKPcX5gu7r01+4o7d9f+YvrOowJ4qF9AWMTuAkcOGCLxRq/tM8wk0DqyjCPIsVdUBi/lcLEuLnsz/Qnb+j5719Wrrkm0z9gJycpnZK3X5L2a/6jj1TTabK2+TDBLCbQt34hf/bZyTVncLkcxt0FqA9pUBr5CTk8bPjIsK+YpJWzb1DE5g1Ko1CQCy+if7hlsTG0c0flrX+a+/xtOWO9jefpdeuyd901cegguy7q+hl/uDFoa6dHf+7/7nn/3e/NVio2Cq5E88VkRDlb1cORI5bzRWIFSOvY1Bwa65wwo1rFScvs57d2s1K//W1FhNJpvXp19vv3LUokzV//5cFvbqtkMgogpUgpKAVmCVPDukCN4fvvL11woZvO2HkcSRMnMAGNjtW4UvKZKe5hmg8xHshiv7IxwU03d+S6UgAeerB45tokM/m+PfXU7Be/1H/uuZnz3qQTbq1YDCbGzcSELeRlahKVCnwf1oKYmCnbxo8/Xu3r4zPW6HJZQmIWjGjh58KE1f508FowSMW3aU0T4/aDW9xzNrRbg6oXPPVk5Stf7QegNVuLhKMve1/nZe/rOHy4uu+F4MB+f3g4GB+TQt6MjdtCXqampFKmqZoYw8/ulauuHK35lEhgnnDU5AmpWiUdBLplhJ0rHIZZrudhcMh+7JrOsFD6/vfyK1cl+/qSxohS0VkaA2bu70/39+P8C+KWb8slWyyYsVE7MuK/8rL//PP+jif9Q4egFGnNzGLtrHIiLr54sK+UrfZ95tmlxoIHwopKeXPNtcmOnGsMJif9e39Q/Oq2gYZyKawARWBjCQoTEatMRmUyTv8AVkdlGtVqwdM7K/ffV/rpw55YlU7DD9DSmGPaLp4XsDXAQi6o4QlBDf0DeNel7caKUrh16+imTelly5NiG4PRdEEz80c8U/1ZC2NgDFmLREJvPK/ti1/qu+vbnctW2OKkOBpzxM3pExBAEHmEuYMXYXaBy4ypkmy+KNnZ5Sqm7T8rPrOnct31PdZGxB2nDyECM6ZdU8iPGCNnn9P+L99ZvHadTJWgFM0VUkUEBN9nnsfpzoAJs6tVpezmzS5A+/ZVbr5p9HNbe11XxwGK32MRgZmUImPQkXPv2Laof8B4NTBhrryGQMYQg2Rew7fxX4ng++jqlnM3po4e8z/9yZHPbe05fXW2oU4/kaUUggBdXclrr2uvVgzx3LUUwYI4wkEahU2xi9Hn0CgqFbtmjVMomk989OCmzW42S7uenioUvDA81RGAE+RBBH/0luzKlVytylymLCKA6JCwGEwiMWbi2EmkfMkkjYzYK98/OjLivPKyufsbY1pzVxc2X+Re/eH2/oGUsaKYToQBIliDhKvXrnd+97taMtk61ScKjXiWBVMIgwEWaFGdMcMEGB6WwSG7ZKl32mrb2+s4ji4U1L/9a+2y9xx9+KEJxTj+jGDudBMAnbRMiwigAJ4mrGGbcCy9lDkeFRmJUpialFWr6JpPJE5ars7b5G7YmDx2zColSklXN1fL+vprC/ffN67UifMgADo6ZgDJVqZMBGJQPQi0tOaIAWZUqugfxGdvzu7ZE/T1J4aGEoUJVKsRthQEoh1pa0t89jNTjz9eUGpW/Pr9ludBAJrtZhrcJtMs9yEtNQeACFtjt93Zee8PSmedlTx21KxZkzp4UBwHYmdQYSLrOM7ffyY/PlY9DgxgAfc9fMgQWKSu1U35HISbPA81+36lKT9hrv2rtOPynt21N1+UfuEF/w2nuaPHgoZYYy2SSRl+Rd22dYxIfu9DYAZgf/sbXzsQsXPklILZlkEteSCC52HJSbLlqty2f8xfcmn7wUOmvU27bqIl/BgE0pGjB34UbP/ZpFJouWehhBcAhoe9554zySTVaW2NssRotbE/mfZIpBSVp8zlH0gDtOMp7x0Xp/fu9vr6GEAmy62kA2slmdS3fqFQLHjE9GoVyVoQ4aEHy+Nj5DgLxEeeFj41hTAAQhDfl1ynXPqetr17q4kkFi1yDw/bXKcDoLeXAhMFmtn1EJJJvHyAbr9tjEle1SGIgAhTk7X/+F4lnebYvS3jWcgA1YH/uAoJIMSolOXMtboj5+54srpkMCFClYp1EgRgxSpHrMRhzbgi5XJ877/7D/5nXmsywfEyYAyYcfddE/v3S72YbtJqqjPFYW8k5klDFYoiGjEZIxvOTYrgpRdrS5fpsB8Rbj/tNNdJYC4HZ62kMuqWm4ovPF9S+riMIQigNR5/rHDPP3sdHTzXLTNpsmCBoC8WTgKr17hEGBuzvb0MwE1yzRMAp5zqdi+CX2tdvolAa1Sq+hMfHx0erihFUVttTupFaxw4ULnxUwWlNJG0iM4NAYrAYYOqyWNbwBLBGGSzNDioAQS+yWQcAJ05TIz7AHp6nNWn62pVmOc0x1RShofVR68e3b+/pDWJwJjGXpM1MAZa0/795Y9/5NjEBLsJO9sFNzqiEFmKjHguLDHs3GSz0tZBcUPqG0gcORKmCvSmTcnQjokir9Ws09kMXj5AH7p89IEfjhMZpSINnAaLwApK2Z9vz1/1wbFDBzmTac7eqCEXClsQALSE0Xp22Ix3/hIuaYfD5oJXrQFYvsI5OhJYa5n5/AuSua7JIAjrCprLLtNplKb0jX879cP7yu96T2bDhtTivkQopnze37u7cv+9pe3bA0frdFrm0bQ4nVG7lmJ12pxRXQAgl3MmxgnAihWO59nh4drQUHJoKLV2nf7VI5LNwoqdx7coLR0deucOefKJye6eyYEBzmTY82RkxIwcgbXc1qZEWmJbXM9x4r0VIiGIXrBhEQRB6A36B/jA/gBAR4fT2aX37vGGhpIA/fFbUo9sn2R27LyZgwiCwGYyIFLVMn7znFhrmElrzmSJMI+bahGJQ04UEwMyTy3LLOUylysC4ORT3MOHI39++mr38ccq4edNm9LdPeTX5HhqYmthjLCSVAqZDKVS0BrWiDEyd3Uuc112HMMicyaNImCmclkmCwbAqlWJfN5MTQUAzt2YemZvTcSKYFFvYu06p1Kxx1/Uh5hK+DdTjryalCPsEoAtzw/lMaNawciIAdA/oAU8POwDWLsuWS7bl170Q5z3/MgXnVAlOW9q3ZgjR61EYl4IixQTYHTUAmhr09msHDhQA9De7gwOqB1PlsNt689OZrNy4pXk8TMQIkvaUcxEc7m/+nHn85HX7+7RrxyIzGDNmcmdO6uhWi9b5ixZyrWaHD+29eorTNtAlQgSDrN27PyjDwKplPzwS28vHzsaMXDmWvfFfT5gReA46pRTnZonTK8R9a17c8kksZuUhbAcUioSbEe7UyhEV0/+g8TUlM3nA6UA0KlvcKyV+Q/zhDCKVuqdbQN3tCtr5vekaM9FTfxsFp4XTbcsXqyVouFD0YGsWOloB6/bCpHwrm7NvYspCOaENQVQCoP9EQPaMb4nod67LqczdHRaowYHVSqN18yOWzCQSEhfn+IVKx1j5+yrmQBtbVi+MjGdXTOreh5CqaTKT0Q7u7oTbVkyBq+VL53tggKD9g4aGnL4jDNczc12zAAzo1aT5Su5fyCq3yenKJkkAGEhlkpJpRJpVDpN2axYI6+D+Ing12RoiepZpHn9WW5nZzi302j4zFT1ZOMbXYBrNbFW8oWgrSME8mGtsDImgLUSBKIVJ1Pa4vU4AWaqebJ+fYJIcV+/e+Y6XSnL7JlAAcQYpFP2T96WYaZUiplpfMwsW+YyUzJFzOTXVLYNzOQ4xIq1Enk9DgBWyE3KRX+YBEgDfOl7sz99OD89L1TvbItfk8WLeXLS/58dxcCIUnLwYOD7ZtfTRa9qHQdjY3Yi7+/ZNVn1LJFMTopivNY8MKM0ZdefpVafkRIBWSvW2i1XHNq9i7LZhlJIQjMIfBtOLLpJDcDzTDjsmXA1kXieEUsguK46Hv2hE8McteZ83r/z7twFmzuMQTRMuHtX8crLx1LpBJpqG6L4BBriraSwncEUoUr/L92N+Vc4cvaOd/Jttw+Eo37MDGuxdl37tddnJ8YCpRunVsP2W/gXpoHxrwCszPr6mlJfnMTJJ5sb/667Du1w1LYwuOrq7i0f1kdHAkWsNM0NyOB1cZSNuad2KJ9Hf19wx9d7OjvdelE/PfgKiAWzvfNrx+7cVmXSmWwo7NfcKInQPAE9uxtLtRoVi/6Gc3nrbd2DQ+n48O6sO0OteuKxwle+XNi7W4i064rWFHrYaSxEFsy8KNZdC8VTn9ppmVdOyzCW/0s4Xw7Pk5pvBgbo8g8kP3RVl9a69ehxHA9kJmv9X2wv/eSh6q9/XRs9hmoVJqAIgAmB4BDdo5nvqM9Gh9MU0gDIRjPeEr2RqD7KTmHTN2I1HK9TCq6LXI5WrFIXXOi+9a2Z7p5kHfqdne7PO35fLtVeeSU4POwfOyoT+Vp+ggv5wPd9IgIhCFCpqKAGkVmClekDkBASIYiAIcxMBDcprhsQkQVIYC15HhvDjiNuIsi0OYt79dKT1JKlemjI6ep2w0eFoG+zm/4/cbk4q2zGq44AAAAASUVORK5CYII=', name: 'Verseau' },
    // Poissons
    { bg: '#0058b0', bg2: '#003f80', light: '#ddeeff', border: '#3377cc',
      img: 'data:image/png;base64,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', name: 'Poissons' },
    // Bélier
    { bg: '#3ff53c', bg2: '#1cc41a', light: '#e8fde8', border: '#66f564',
      img: 'data:image/png;base64,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', name: 'Bélier' },
    // Taureau
    { bg: '#fe6f04', bg2: '#d45600', light: '#fff0e5', border: '#ff9944',
      img: 'data:image/png;base64,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', name: 'Taureau' },
    // Gémeaux
    { bg: '#16a4f4', bg2: '#0a7bc0', light: '#e0f4ff', border: '#55bdf9',
      img: 'data:image/png;base64,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', name: 'Gémeaux' },
    // Cancer
    { bg: '#20c3d4', bg2: '#0e96a5', light: '#e0f9fb', border: '#50d8e6',
      img: 'data:image/png;base64,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', name: 'Cancer' },
    // Lion
    { bg: '#fc582d', bg2: '#d03b14', light: '#fff0ec', border: '#fd8060',
      img: 'data:image/png;base64,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', name: 'Lion' },
    // Vierge
    { bg: '#ea37d9', bg2: '#be1eae', light: '#fce8fb', border: '#f06ee4',
      img: 'data:image/png;base64,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', name: 'Vierge' },
    // Balance
    { bg: '#ca9225', bg2: '#9e6e0c', light: '#fdf3dc', border: '#ddb94d',
      img: 'data:image/png;base64,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', name: 'Balance' },
    // Scorpion
    { bg: '#fc4c3b', bg2: '#c82c1c', light: '#ffeceb', border: '#fd7a6d',
      img: 'data:image/png;base64,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', name: 'Scorpion' },
    // Sagittaire
    { bg: '#8744eb', bg2: '#6620c7', light: '#f0e8ff', border: '#aa77f0',
      img: 'data:image/png;base64,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', name: 'Sagittaire' },
  ];

  getGroupColor(mois: string): { bg: string; bg2: string; light: string; border: string; img: string; name: string } {
    const monthNum = parseInt(mois.split('-')[1], 10);
    return this.ZODIAC_BY_MONTH[(monthNum - 1) % 12];
  }

  getMonthIcon(mois: string): string {
    return ''; // unused, use getGroupColor().img instead
  }

  // État collapse : Set des mois réduits
  collapsedGroups = new Set<string>();

  toggleGroup(mois: string): void {
    if (this.collapsedGroups.has(mois)) {
      this.collapsedGroups.delete(mois);
    } else {
      this.collapsedGroups.add(mois);
    }
    this.cdr.detectChanges();
  }

  isGroupCollapsed(mois: string): boolean {
    return this.collapsedGroups.has(mois);
  }

  collapseAll(): void {
    this.getGroupedByMoisAnnee.forEach(g => this.collapsedGroups.add(g.mois));
    this.cdr.detectChanges();
  }

  expandAll(): void {
    this.collapsedGroups.clear();
    this.cdr.detectChanges();
  }

  get allCollapsed(): boolean {
    return this.getGroupedByMoisAnnee.length > 0 &&
           this.getGroupedByMoisAnnee.every(g => this.collapsedGroups.has(g.mois));
  }

  // Groupement
  get getGroupedByMoisAnnee(): { mois: string; label: string; annee: string; jours: JourTravailDto[]; totalJours: number; totalRevenu: number }[] {
    const map = new Map<string, JourTravailDto[]>();
    for (const jour of this.filteredJours) {
      const key = jour.mois;
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(jour);
    }
    return Array.from(map.entries())
      .sort(([a], [b]) => b.localeCompare(a))
      .map(([mois, jours]) => ({
        mois,
        label: this.formatMois(mois),
        annee: mois.split('-')[0],
        jours,
        totalJours: jours.reduce((s, j) => s + (j.nombreJours || 0), 0),
        totalRevenu: jours.reduce((s, j) => s + ((j.nombreJours || 0) * (j.tjm || 0)), 0)
      }));
  }

  // ✅ NOUVEAU
  isSorted(column: string): boolean {
    return this.sortColumn === column;
  }

  // ✅ NOUVEAU
  getSortIcon(column: string): string {
    if (this.sortColumn !== column) {
      return 'bi-arrow-down-up'; // Icône neutre
    }
    return this.sortDirection === 'asc' ? 'bi-arrow-up' : 'bi-arrow-down';
  }

  openModal(jour?: JourTravailDto) {
    this.isEditMode = !!jour;
    this.closeAllDropdowns();

    // Vérifier si un mois est sélectionné ET verrouillé
  if (this.selectedJour.mois && this.isMoisVerrouille(this.selectedJour.mois)) {
    // Si User, bloquer l'ouverture
    if (!this.canManage()) {
      this.showNotification('error', `Le mois ${this.formatMois(this.selectedJour.mois)} est verrouillé. Vous ne pouvez pas le modifier.`);
      return;
    }
  }

    if (jour) {
      this.selectedJour = { ...jour };
      
      // ✅ FIX : S'assurer que le prestataireId est bien défini
      if (!this.selectedJour.prestataireId && jour.prestataire?.id) {
        this.selectedJour.prestataireId = jour.prestataire.id;
      }
    } else {
      this.selectedJour = {
        nombreJours: 1,
        tjm: 0
      };
      
      // ✅ AJOUT : Si User, pré-remplir avec son prestataireId
      if (!this.canManage()) {
        const userPrestataireId = this.getCurrentUserPrestataireId();
        if (userPrestataireId) {
          this.selectedJour.prestataireId = userPrestataireId;
        }
      }
      
      // ✅ FIX : Ne PAS mettre tjmFromContratFound = true par défaut !
      this.tjmFromContratFound = false;
      this.contratQuotaInfo = {
        nombreJoursContrat: 0,
        joursDejaUtilises: 0,
        joursRestants: 0,
        pourcentageUtilise: 0
      };
    }

    this.isModalOpen = true;

    // ✅ FIX : Appeler updateTjmAndQuotaFromContrat TOUJOURS (même si juste prestataireId)
    if (this.selectedJour.prestataireId) {
      this.updateTjmAndQuotaFromContrat();
    }

    this.cdr.detectChanges();
  }
editJour(jour: JourTravailDto) {
  this.closeAllDropdowns();
  
  // ✅ Vérifier si le mois est verrouillé
  if (this.isMoisVerrouille(jour.mois)) {
    // Si User, bloquer l'édition
    if (!this.canManage()) {
      this.showNotification('error', `Le mois ${this.formatMois(jour.mois)} est verrouillé. Vous ne pouvez pas le modifier.`);
      return;
    }
    
    // Si Admin/Manager, afficher un avertissement mais autoriser
    this.showNotification('warning', `⚠️ Attention : Le mois ${this.formatMois(jour.mois)} est verrouillé. Vous pouvez le modifier car vous êtes Admin/Manager.`);
  }
  
  // Reste du code existant...
  this.selectedJour = { ...jour };
  this.isEditMode = true;
  this.isModalOpen = true;
}
  closeModal() {
    this.isModalOpen = false;
    this.selectedJour = {};
    this.showMonthPicker = false;
    this.cdr.detectChanges();
  }

  onPrestataireChange() {
    if (this.selectedJour.prestataireId && this.selectedJour.mois) {
      this.updateTjmAndQuotaFromContrat();
    }
  }

  private updateTjmAndQuotaFromContrat() {
    if (!this.selectedJour.prestataireId || !this.selectedJour.mois) return;

    const contrat = this.findActiveContratForMonth(
      this.selectedJour.prestataireId,
      this.selectedJour.mois
    );

    if (contrat) {
      // ✅ FIX : Gérer le TJM pour Admin/Manager uniquement
      if (contrat.tjm != null && contrat.tjm > 0) {
        this.selectedJour.tjm = contrat.tjm;
        this.tjmFromContratFound = true;
        this.tjmSourceContratId = contrat.id;
      } else {
        // Pour User : pas de TJM dans le contrat, mais on peut quand même calculer le quota
        this.selectedJour.tjm = 0;
        this.tjmFromContratFound = false;
        this.tjmSourceContratId = undefined;
      }

      // ✅ Calculer le quota même sans TJM (pour User)
      if (this.isMonthInContratPeriod(this.selectedJour.mois, contrat)) {
        const joursDispo = contrat.nombreJours || 0;
        const joursUtilises = contrat.joursUtilises || 0;
        const joursRestants = Math.max(0, joursDispo - joursUtilises);
        const pourcentageUtilise = joursDispo > 0 ? (joursUtilises / joursDispo) * 100 : 0;

        this.contratQuotaInfo = {
          nombreJoursContrat: joursDispo,
          joursDejaUtilises: joursUtilises,
          joursRestants,
          pourcentageUtilise
        };
      } else {
        this.contratQuotaInfo = {
          nombreJoursContrat: 0,
          joursDejaUtilises: 0,
          joursRestants: 0,
          pourcentageUtilise: 0
        };
      }
    } else {
      // Aucun contrat trouvé
      this.selectedJour.tjm = 0;
      this.tjmFromContratFound = false;
      this.tjmSourceContratId = undefined;
      
      this.contratQuotaInfo = {
        nombreJoursContrat: 0,
        joursDejaUtilises: 0,
        joursRestants: 0,
        pourcentageUtilise: 0
      };
    }

    this.cdr.detectChanges();
  }

  private isMonthInContratPeriod(moisYYYYMM: string, contrat: ContratDto): boolean {
    const [yStr, mStr] = moisYYYYMM.split('-');
    const y = Number(yStr);
    const m = Number(mStr);

    if (!y || !m || m < 1 || m > 12) return false;

    const monthStart = new Date(y, m - 1, 1, 0, 0, 0, 0);
    const monthEnd = new Date(y, m, 0, 23, 59, 59, 999);

    const start = this.safeDate(contrat.dateDebut);
    const end = contrat.dateFin ? this.safeDate(contrat.dateFin) : null;

    if (!start) return false;

    const cond1 = start.getTime() <= monthEnd.getTime();
    const cond2 = !end || end.getTime() >= monthStart.getTime();
    
    return cond1 && cond2;
  }

  private findActiveContratForMonth(prestataireId: string, moisYYYYMM: string): ContratDto | null {
    const [yStr, mStr] = moisYYYYMM.split('-');
    const y = Number(yStr);
    const m = Number(mStr);

    if (!y || !m || m < 1 || m > 12) return null;

    const monthStart = new Date(y, m - 1, 1, 0, 0, 0, 0);
    const monthEnd = new Date(y, m, 0, 23, 59, 59, 999);

    const matches = (this.contrats ?? [])
      .filter(c => c.prestataireId === prestataireId)
      .filter(c => {
        const start = this.safeDate(c.dateDebut);
        const end = c.dateFin ? this.safeDate(c.dateFin) : null;

        if (!start) return false;

        const cond1 = start.getTime() <= monthEnd.getTime();
        const cond2 = !end || end.getTime() >= monthStart.getTime();
        return cond1 && cond2;
      })
      .sort((a, b) => {
        const da = this.safeDate(a.dateDebut)?.getTime() ?? 0;
        const db = this.safeDate(b.dateDebut)?.getTime() ?? 0;
        return db - da;
      });

    return matches.length ? matches[0] : null;
  }

  private safeDate(value: any): Date | null {
    if (!value) return null;
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

  saveJour() {
    if (this.selectedJour.mois && this.isMoisVerrouille(this.selectedJour.mois)) {
      if (!this.canManage()) {
        this.showNotification('error', `Le mois ${this.formatMois(this.selectedJour.mois)} est verrouillé. Vous ne pouvez pas le modifier.`);
        return;
      }
    }
    
    // Validation basique
    if (!this.selectedJour.prestataireId || !this.selectedJour.mois || !this.selectedJour.nombreJours) {
      this.showNotification('warning', 'Veuillez remplir tous les champs obligatoires');
      return;
    }

    // ✅ FIX 3 : Vérifier doublons (même prestataire + même mois)
    const existeDeja = this.joursTravail.some(j => 
      j.prestataireId === this.selectedJour.prestataireId && 
      j.mois === this.selectedJour.mois &&
      j.id !== this.selectedJour.id  // Exclure le jour en cours de modification
    );

    if (existeDeja) {
      this.showNotification('error', `Un jour de travail existe déjà pour ce prestataire sur ${this.formatMois(this.selectedJour.mois!)}`);
      return;
    }

    // ✅ FIX FINAL : Pas de validation bloquante frontend
    // Le backend gère TOUTE la validation (TJM + quota)
    
    // ℹ️ Avertissement quota (optionnel, ne bloque PAS)
    if (this.contratQuotaInfo.nombreJoursContrat > 0) {
      const nombreJoursSaisi = Number(this.selectedJour.nombreJours);
      const joursDisponibles = this.contratQuotaInfo.joursRestants;
      
      if (nombreJoursSaisi > joursDisponibles) {
        console.warn(`⚠️ Quota dépassé : ${nombreJoursSaisi}j demandés, ${joursDisponibles}j disponibles`);
        // Continue quand même, le backend validera
      }
    }

    // Enregistrement
    if (this.isEditMode && this.selectedJour.id) {
      this.jourTravailService.update(this.selectedJour.id, this.selectedJour as JourTravailDto).subscribe({
        next: () => {
          this.closeModal();
          this.loadJoursTravail();
          this.showNotification('success', 'Jour de travail modifié avec succès !');
          
          this.cagnotteService.refreshCagnotte();
        },
        error: (err) => {
          const errorMsg = err?.error?.message || err?.message || 'Erreur lors de la modification';
          this.showNotification('error', errorMsg);
        }
      });
    } else {
      this.jourTravailService.create(this.selectedJour as JourTravailDto).subscribe({
        next: () => {
          this.closeModal();
          this.loadJoursTravail();
          this.showNotification('success', 'Jour de travail créé avec succès !');
          
          this.cagnotteService.refreshCagnotte();
        },
        error: (err) => {
          const errorMsg = err?.error?.message || err?.message || 'Erreur lors de la création';
          this.showNotification('error', errorMsg);
        }
      });
    }
  }
  getMoisVerrouilleInfo(mois: string): string {
    const moisVerrouille = this.moisVerrouilleService.getMoisVerrouille(mois);
    if (moisVerrouille) {
      return `🔒 Verrouillé par ${moisVerrouille.verrouilleParUserName || 'Admin'}`;
    }
    return '';
  }
  confirmDelete(jour: JourTravailDto) {
    this.closeAllDropdowns();
    this.jourToDelete = jour;
    this.showDeleteConfirm = true;
    this.cdr.detectChanges();
  }
  private loadCagnotte(): void {
    
  }
  deleteJour() {
    if (this.jourToDelete && this.jourToDelete.id) {
      this.jourTravailService.delete(this.jourToDelete.id).subscribe({
        next: () => {
          this.loadJoursTravail();
          this.showDeleteConfirm = false;
          this.jourToDelete = null;
          this.showNotification('success', 'Jour de travail supprimé avec succès !');
          this.loadCagnotte();
          this.cagnotteService.refreshCagnotte();
          this.cdr.detectChanges();
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la suppression');
          this.showDeleteConfirm = false;
          this.jourToDelete = null;
          this.cdr.detectChanges();
        }
      });
    }
  }

  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notifService.show(type, message);
  }

  formatMois(mois: string): string {
    const [annee, moisNum] = mois.split('-');
    const date = new Date(parseInt(annee), parseInt(moisNum) - 1, 1);
    return date.toLocaleDateString('fr-FR', { year: 'numeric', month: 'long' });
  }

  getInitials(nom?: string, prenom?: string): string {
    const n = nom?.charAt(0).toUpperCase() || '';
    const p = prenom?.charAt(0).toUpperCase() || '';
    return n + p || '??';
  }

  // ✅ URL image avatar prestataire
  getAvatarUrl(imageUrl?: string): string | null {
      if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  toggleMonthPicker(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    
    this.showMonthPicker = !this.showMonthPicker;

    if (this.selectedJour.mois) {
      const [year] = this.selectedJour.mois.split('-');
      this.selectedYear = parseInt(year);
    }

    if (this.showMonthPicker) {
      setTimeout(() => {
        const closeHandler = (e: Event) => {
          const target = e.target as HTMLElement;
          if (!target.closest('.custom-month-picker')) {
            this.closeMonthPicker();
            document.removeEventListener('click', closeHandler);
          }
        };
        document.addEventListener('click', closeHandler);
      }, 0);
    }
  }

  closeMonthPicker() {
    this.showMonthPicker = false;
    this.cdr.detectChanges();
  }

  changeYear(delta: number, event?: Event) {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    this.selectedYear += delta;
  }

  selectMonth(monthNumber: number, event?: Event) {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    
    const monthStr = String(monthNumber).padStart(2, '0');
    this.selectedJour.mois = `${this.selectedYear}-${monthStr}`;
    this.showMonthPicker = false;

    this.updateTjmAndQuotaFromContrat();
    this.cdr.detectChanges();
  }

  isMonthSelected(monthNumber: number): boolean {
    if (!this.selectedJour.mois) return false;
    const [year, month] = this.selectedJour.mois.split('-');
    return parseInt(year) === this.selectedYear && parseInt(month) === monthNumber;
  }

  formatMoisInput(mois: string): string {
    if (!mois) return '';
    const [year, month] = mois.split('-');
    const monthName = this.moisList.find(m => m.value === parseInt(month))?.label || '';
    return `${monthName} ${year}`;
  }

  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  // ========== MENU 3 POINTS CARTES ==========

  toggleCardMenu(jourId: string, event: Event) {
    event.stopPropagation();
    if (this.activeCardMenuId === jourId) {
      this.activeCardMenuId = null;
    } else {
      this.activeCardMenuId = jourId;
      
      // Fermer le menu au clic ailleurs
      setTimeout(() => {
        const closeHandler = (e: Event) => {
          const target = e.target as HTMLElement;
          if (!target.closest('.dropdown')) {
            this.closeCardMenu();
            document.removeEventListener('click', closeHandler);
          }
        };
        document.addEventListener('click', closeHandler);
      }, 0);
    }
  }

  closeCardMenu() {
    this.activeCardMenuId = null;
    this.cdr.detectChanges();
  }
  // Ajoute cette méthode :
toggleDropdown(jourId: string, event: Event): void {
  event.stopPropagation();
  
  if (this.openDropdownId === jourId) {
    this.openDropdownId = null;
  } else {
    this.openDropdownId = jourId;
  }
}

// Ajoute cette méthode :
isDropdownOpen(jourId: string): boolean {
  return this.openDropdownId === jourId;
}

// Ajoute cette méthode :
closeAllDropdowns(): void {
  this.openDropdownId = null;
}
//Dans ngOnDestroy(), retire le listener
ngOnDestroy() {
  document.removeEventListener('click', this.closeAllDropdowns.bind(this));
}
}