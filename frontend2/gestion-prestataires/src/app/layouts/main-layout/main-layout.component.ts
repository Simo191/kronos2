import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CagnotteService } from '../../services/cagnotte.service';
import { PermissionService, Permission, Role } from '../../services/permission.service';
import { HasPermissionDirective, HasRoleDirective } from '../../directives/permission.directives';
import { User } from '../../models/user.model';
import { Cagnotte } from '../../models/cagnotte.model';
import { LogoPrestaPro } from '../../components/logo-prestapro/logo-prestapro.component';
import { environment } from '../../../environments/environment';
import { Prestataire } from '../../models/prestataire.model';
import { PrestataireService } from '../../services/prestataire.service';
import { NotificationBellComponent } from '../../components/notification-bell/notification-bell.component';
import { NotificationCenterService } from '../../services/notification-center.service';
 
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LogoPrestaPro,
    HasPermissionDirective,
    FormsModule,
    NotificationBellComponent
],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MainLayoutComponent implements OnInit {
  sidebarCollapsed = false;
  showUserDropdown = false;

  currentUser: User | null = null;

  cagnottes: Cagnotte[] = [];
  selectedCagnotte: Cagnotte | null = null;
  cagnotteActuelle: Cagnotte | null = null;

  currentPrestataire: Prestataire | null = null;

  selectedAnnee: number = new Date().getFullYear();

  Permission = Permission;
  Role = Role;

  // ✅ NOUVEAU : État des sections (ouvertes/fermées)
  sections = {
    operations: true,   // Ouvert par défaut (le plus utilisé)
    finance: true,      // Ouvert par défaut
    config: false,      // Fermé par défaut (moins utilisé)
    admin: false        // Fermé par défaut (moins utilisé)
  };

  constructor(
    private authService: AuthService,
    private cagnotteService: CagnotteService,
    private permissionService: PermissionService,
    private prestataireService: PrestataireService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private notifCenter: NotificationCenterService
  ) {
    this.authService.currentUser.subscribe({ 
      next: (u) => {
        this.currentUser = u;
        if (u?.id) {
          this.prestataireService.getAll().subscribe(list => {
            this.currentPrestataire = list.find(p => p.userId === u.id) ?? null;
            this.cdr.markForCheck();
          });
        }
        this.cdr.markForCheck();
      }
    });
  }

  ngOnInit() {
    // Charger l'état des sections depuis localStorage
    this.loadSectionsState();

    //   Démarre le polling des notifications (cloche header) — refresh auto toutes les 5 min
    this.notifCenter.startAutoRefresh();
    
    // Écouter les cagnottes
    this.cagnotteService.cagnottes$.subscribe(list => {
      this.cagnottes = (list ?? []).slice().sort((a, b) => (b.annee ?? 0) - (a.annee ?? 0));
      
      if (this.cagnottes.length > 0 && !this.selectedCagnotte) {
        this.initializeSelectedAnnee();
      }
      
      this.cdr.markForCheck();
    });

    // Écouter la cagnotte sélectionnée
    this.cagnotteService.selectedCagnotte$.subscribe(c => {
      this.selectedCagnotte = c;
      
      if (c && c.annee) {
        this.selectedAnnee = c.annee;
      }
      
      this.cdr.markForCheck();
    });

    // Écouter la cagnotte actuelle
    this.cagnotteService.cagnotteActuelle$.subscribe(c => {
      this.cagnotteActuelle = c;
      this.cdr.markForCheck();
    });
  }

  // ✅ NOUVEAU : Toggle section collapse
  toggleSection(section: 'operations' | 'finance' | 'config' | 'admin') {
    this.sections[section] = !this.sections[section];
    this.saveSectionsState();
    this.cdr.markForCheck();
  }

  // ✅ NOUVEAU : Sauvegarder l'état des sections dans localStorage
  private saveSectionsState() {
    localStorage.setItem('menuSections', JSON.stringify(this.sections));
  }

  // ✅ NOUVEAU : Charger l'état des sections depuis localStorage
  private loadSectionsState() {
    const saved = localStorage.getItem('menuSections');
    if (saved) {
      try {
        this.sections = JSON.parse(saved);
      } catch (e) {
        console.error('Erreur chargement état sections:', e);
      }
    }
  }

  initializeSelectedAnnee() {
    const storedAnnee = localStorage.getItem('selectedCagnotteAnnee');
    
    if (storedAnnee) {
      const annee = parseInt(storedAnnee, 10);
      const cagnotteExists = this.cagnottes.find(c => c.annee === annee);
      
      if (cagnotteExists) {
        this.selectedAnnee = annee;
        this.cagnotteService.selectYear(annee);
        return;
      }
    }
    
    const currentYear = new Date().getFullYear();
    const cagnotteActuelle = this.cagnottes.find(c => c.annee === currentYear);
    
    if (cagnotteActuelle) {
      this.selectedAnnee = currentYear;
      this.cagnotteService.selectYear(currentYear);
    } else if (this.cagnottes.length > 0) {
      const firstAnnee = this.cagnottes[0].annee;
      this.selectedAnnee = firstAnnee ?? currentYear;
      this.cagnotteService.selectYear(this.selectedAnnee);
    }
  }

  onSelectCagnotteYear(anneeStr: string) {
    const annee = Number(anneeStr);
    if (!annee) return;
        
    localStorage.setItem('selectedCagnotteAnnee', annee.toString());
    
    this.selectedAnnee = annee;
    this.cagnotteService.selectYear(annee);
    
    this.cdr.markForCheck();
  }

  onAnneeChange() {
    this.onSelectCagnotteYear(this.selectedAnnee.toString());
  }

  canViewCagnottes(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES);
  }

  get budgetClass(): string {
    if (!this.selectedCagnotte) return 'text-secondary';
    const budgetRestant = this.selectedCagnotte.budgetRestant ?? 0;
    return budgetRestant >= 0 ? 'text-success' : 'text-danger';
  }

  isActuelle(annee?: number): boolean {
    return !!annee && !!this.cagnotteActuelle && this.cagnotteActuelle.annee === annee;
  }

  getProgressPercentage(): number {
    if (!this.selectedCagnotte || !this.selectedCagnotte.budgetAnnuel) return 0;
    
    const budgetAnnuel = this.selectedCagnotte.budgetAnnuel ?? 0;
    const budgetConsomme = this.selectedCagnotte.budgetConsomme ?? 0;
    
    if (budgetAnnuel === 0) return 0;
    
    const percentage = (budgetConsomme / budgetAnnuel) * 100;
    return Math.min(Math.max(percentage, 0), 100);
  }

  formatMontant(montant: number | undefined): string {
    const amount = montant ?? 0;
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(amount);
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  toggleUserDropdown() {
    this.showUserDropdown = !this.showUserDropdown;
    
    if (this.showUserDropdown) {
      setTimeout(() => {
        const closeHandler = (e: Event) => {
          const target = e.target as HTMLElement;
          if (!target.closest('.dropdown')) {
            this.showUserDropdown = false;
            this.cdr.markForCheck();
            document.removeEventListener('click', closeHandler);
          }
        };
        document.addEventListener('click', closeHandler);
      }, 0);
    }
  }

  logout() {
    this.showUserDropdown = false;
    this.notifCenter.clear(); //   purge le centre de notifications à la déconnexion
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  closeUserDropdown(): void {
    this.showUserDropdown = false;
  }
  getAvatarUrl(imageUrl?: string): string | null {
    if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  // ✅ NOUVEAU : Vérifier si l'utilisateur a accès à au moins 1 item de chaque section
  canViewOperations(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_PLANNING) ||
           this.permissionService.hasPermission(Permission.VIEW_PRESTATAIRES) ||
           this.permissionService.hasPermission(Permission.VIEW_CONTRATS) ||
           this.permissionService.hasPermission(Permission.VIEW_JOURS_TRAVAIL);
  }

  canViewFinance(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES) ||
           this.permissionService.hasPermission(Permission.VIEW_TYPE_CONTRAT);
  }

  canViewParametres(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_TYPE_PLANNING) ||
           this.permissionService.hasPermission(Permission.VIEW_TYPE_CONTRAT);
  }

  canViewAdministration(): boolean {
    return this.permissionService.hasPermission(Permission.VIEW_USERS) ||
           this.permissionService.hasPermission(Permission.VIEW_ROLES);
  }
}