import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleService, RoleDto } from '../../services/role.service';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit {
  roles: RoleDto[] = [];
  selectedRole: Partial<RoleDto> = {};
  
  isLoading = false;
  isModalOpen = false;
  isEditMode = false;
  errorMessage = '';
  successMessage = '';
  
  // Filtres et recherche
  searchTerm = '';
  viewMode: 'grid' | 'list' = 'grid';
  
  // Stats
  totalRoles = 0;
  activeRoles = 0;
  totalUsers = 0;
  
  constructor(private roleService: RoleService,    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadRoles();
  }

  loadRoles() {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.roleService.getAll().subscribe({
      next: (data) => {
        this.roles = data;
        this.calculateStats();
        this.isLoading = false;
        this.cdr.detectChanges();

      },
      error: (error) => {
        this.errorMessage = 'Erreur lors du chargement des rôles';
        this.isLoading = false;
        console.error('Erreur:', error);
        this.cdr.detectChanges();

      }
    });
  }

  calculateStats() {
    this.totalRoles = this.roles.length;
    this.activeRoles = this.roles.filter(r => r.isActive !== false).length;
    this.totalUsers = this.roles.reduce((sum, role) => sum + (role.userCount || 0), 0);
  }

  openModal(role?: RoleDto) {
    this.errorMessage = '';
    this.successMessage = '';
    
    if (role) {
      this.isEditMode = true;
      this.selectedRole = { ...role };
    } else {
      this.isEditMode = false;
      this.selectedRole = {
        name: '',
        description: '',
        isActive: true
      };
    }
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedRole = {};
    this.errorMessage = '';
  }

  saveRole() {
    // Validation
    if (!this.selectedRole.name || !this.selectedRole.description) {
      this.errorMessage = 'Veuillez remplir tous les champs obligatoires';
      return;
    }

    // Validation du nom (pas de caractères spéciaux)
    const nameRegex = /^[a-zA-ZÀ-ÿ0-9\s\-_]+$/;
    if (!nameRegex.test(this.selectedRole.name)) {
      this.errorMessage = 'Le nom ne doit contenir que des lettres, chiffres, espaces, tirets et underscores';
      return;
    }

    if (this.selectedRole.description.length < 10) {
      this.errorMessage = 'La description doit contenir au moins 10 caractères';
      return;
    }

    if (this.isEditMode && this.selectedRole.id) {
      this.roleService.update(this.selectedRole.id, this.selectedRole as RoleDto).subscribe({
        next: () => {
          this.successMessage = 'Rôle modifié avec succès';
          this.loadRoles();
          this.closeModal();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Erreur lors de la modification';
          console.error('Erreur:', error);
        }
      });
    } else {
      this.roleService.create(this.selectedRole as RoleDto).subscribe({
        next: () => {
          this.successMessage = 'Rôle créé avec succès';
          this.loadRoles();
          this.closeModal();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Erreur lors de la création';
          console.error('Erreur:', error);
        }
      });
    }
  }

  deleteRole(id: string, roleName: string) {
    if (confirm(`Êtes-vous sûr de vouloir supprimer le rôle "${roleName}" ? Cette action est irréversible.`)) {
      this.roleService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Rôle supprimé avec succès';
          this.loadRoles();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Erreur lors de la suppression';
          console.error('Erreur:', error);
        }
      });
    }
  }

  // Filtres et recherche
  get filteredRoles(): RoleDto[] {
    let filtered = this.roles;

    // Recherche par texte
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(role => 
        role.name.toLowerCase().includes(term) ||
        role.description.toLowerCase().includes(term)
      );
    }

    return filtered;
  }

  toggleViewMode() {
    this.viewMode = this.viewMode === 'grid' ? 'list' : 'grid';
  }

  resetFilters() {
    this.searchTerm = '';
  }

  getRoleIcon(roleName: string): string {
    const name = roleName.toLowerCase();
    if (name.includes('admin')) return 'bi-shield-fill-check';
    if (name.includes('manager') || name.includes('gestionnaire')) return 'bi-person-badge';
    if (name.includes('user') || name.includes('utilisateur')) return 'bi-person';
    if (name.includes('supervisor') || name.includes('superviseur')) return 'bi-eye';
    return 'bi-shield';
  }

  getRoleColor(roleName: string): string {
    const name = roleName.toLowerCase();
    if (name.includes('admin')) return 'danger';
    if (name.includes('manager') || name.includes('gestionnaire')) return 'warning';
    if (name.includes('user') || name.includes('utilisateur')) return 'info';
    if (name.includes('supervisor') || name.includes('superviseur')) return 'success';
    return 'primary';
  }

  exportToCSV() {
    const headers = ['Nom', 'Description', 'Nombre d\'utilisateurs', 'Statut'];
    const data = this.filteredRoles.map(role => [
      role.name,
      role.description,
      role.userCount || 0,
      role.isActive !== false ? 'Actif' : 'Inactif'
    ]);

    const csvContent = [
      headers.join(','),
      ...data.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n');

    const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `roles_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}