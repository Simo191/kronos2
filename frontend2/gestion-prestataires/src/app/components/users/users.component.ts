import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, UserDto } from '../../services/user.service';
import { RoleService, RoleDto } from '../../services/role.service';
import { PrestataireService } from '../../services/prestataire.service';
import { Prestataire } from '../../models/prestataire.model';
import { PermissionService, Permission, Role } from '../../services/permission.service';
import { HasPermissionDirective, HasRoleDirective } from '../../directives/permission.directives';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  users: UserDto[] = [];
  roles: RoleDto[] = [];
  prestataires: Prestataire[] = [];
  selectedUser: Partial<UserDto> = {};
  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  
  showDeleteConfirm = false;
  userToDelete: UserDto | null = null;
  
  searchTerm = '';
  filterRole = '';
  filterStatus = '';
  
  currentPage = 1;
  itemsPerPage = 10;
  viewMode: 'list' | 'cards' = 'cards';
  
  Permission = Permission;
  Role = Role;

  temporaryPassword: string = '';

  // ✅ Notification email bienvenue (remplace confirm natif)
  showWelcomePrompt = false;
  welcomePromptEmail = '';
  welcomePromptPassword = '';
  welcomePromptNom = '';
  
  // ✅ NOUVEAU : Modal mot de passe temporaire
  showTempPasswordModal = false;
  tempPasswordUser: UserDto | null = null;
  newTempPassword = '';
  currentPassword='';
  tempPasswordError = '';
  
  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private prestataireService: PrestataireService,
    private permissionService: PermissionService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    
  ) {}

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
    this.loadPrestataires();
  }

  loadUsers() {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.userService.getAll().subscribe({
      next: (data) => {
        this.users = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage = 'Erreur lors du chargement des utilisateurs';
        this.isLoading = false;
        console.error('Erreur:', error);
        this.cdr.detectChanges();
      }
    });
  }

  loadRoles() {
    this.roleService.getAll().subscribe({
      next: (data) => {
        this.roles = data;
      },
      error: (error) => {
        console.error('Erreur chargement roles:', error);
      }
    });
  }

  loadPrestataires() {
    this.prestataireService.getAll().subscribe({
      next: (data) => {
        this.prestataires = data ?? [];
      },
      error: (error) => {
        console.error('Erreur chargement prestataires:', error);
      }
    });
  }

  get availablePrestataires(): Prestataire[] {
    return this.prestataires.filter(p => 
      !p.userId || p.userId === this.selectedUser.id
    );
  }

  getPrestataireForUser(userId: string): Prestataire | undefined {
    return this.prestataires.find(p => p.userId === userId);
  }

  openModal(user?: UserDto) {
    this.errorMessage = '';
    this.successMessage = '';
    
    if (user) {
      this.isEditMode = true;
      this.selectedUser = { ...user };
      
      const prestataire = this.getPrestataireForUser(user.id!);
      if (prestataire) {
        this.selectedUser.prestataireId = prestataire.id;
      }
    } else {
      this.isEditMode = false;
      
      const defaultRoleId = this.getDefaultRoleId();
      
      this.selectedUser = { 
        isActive: true,
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        roleId: defaultRoleId,
        prestataireId: null
      };
    }
    this.isModalOpen = true;
  }

  getDefaultRoleId(): string {
    const userRole = this.roles.find(r => r.name === 'User');
    return userRole?.id || '';
  }

  canManageUserRoles(): boolean {
    return this.permissionService.canManageUserRoles();
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedUser = {};
    this.errorMessage = '';
  }

  saveUser() {
    if (!this.selectedUser.email || !this.selectedUser.firstName || 
        !this.selectedUser.lastName || !this.selectedUser.roleId) {
      this.showErrorNotification('Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (!this.isEditMode && !this.selectedUser.password) {
      this.showErrorNotification('Le mot de passe est obligatoire');
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.selectedUser.email)) {
      this.showErrorNotification('Email invalide');
      return;
    }

    if (!this.canManageUserRoles() && !this.isEditMode) {
      const userRoleId = this.getDefaultRoleId();
      this.selectedUser.roleId = userRoleId;
    }

    if (!this.isEditMode && this.selectedUser.password) {
      this.temporaryPassword = this.selectedUser.password;
    }

    if ((this.selectedUser as any).prestataireId === 'null' || (this.selectedUser as any).prestataireId === '') {
      this.selectedUser.prestataireId = null;
    }

    if (this.isEditMode && this.selectedUser.id) {
      this.userService.update(this.selectedUser.id, this.selectedUser as UserDto).subscribe({
        next: () => {
          this.showSuccessNotification('Utilisateur modifié avec succès');
          this.loadUsers();
          this.loadPrestataires();
          this.closeModal();
        },
        error: (error) => {
          this.showErrorNotification(error.error?.message || 'Erreur lors de la modification');
          console.error('Erreur:', error);
        }
      });
    } else {
      // ✅ Sauvegarder nom/email AVANT closeModal() qui efface selectedUser
      const createdEmail = this.selectedUser.email!;
      const createdFirstName = this.selectedUser.firstName!;
      const createdLastName = this.selectedUser.lastName!;
      const createdPassword = this.temporaryPassword;

      this.userService.create(this.selectedUser as UserDto).subscribe({
        next: () => {
          this.loadUsers();
          this.loadPrestataires();
          this.closeModal();

          // ✅ Proposer l'envoi email via notification + bouton (pas de confirm natif)
          this.showWelcomeEmailPrompt(createdEmail, createdFirstName, createdLastName, createdPassword);
          
          this.temporaryPassword = '';
        },
        error: (error) => {
          this.showErrorNotification(error.error?.message || 'Erreur lors de la création');
          console.error('Erreur:', error);
        }
      });
    }
  }

  showWelcomeEmailPrompt(email: string, firstName: string, lastName: string, password: string) {
    this.welcomePromptEmail = email;
    this.welcomePromptPassword = password;
    this.welcomePromptNom = `${firstName} ${lastName}`;
    this.showWelcomePrompt = true;
    this.cdr.detectChanges();
  }

  confirmWelcomeEmail() {
    this.sendWelcomeEmail(this.welcomePromptEmail, this.welcomePromptPassword);
    this.showWelcomePrompt = false;
    this.showSuccessNotification('Email de bienvenue envoyé avec succès');
    this.cdr.detectChanges();
  }

  cancelWelcomeEmail() {
    this.showWelcomePrompt = false;
    this.cdr.detectChanges();
  }

  sendWelcomeEmail(userEmail: string, password: string) {
    const appUrl = window.location.origin;
    const loginUrl = `${appUrl}/login`;
    
    const subject = 'Bienvenue - Vos identifiants de connexion';
    
    const body = `Bonjour,

Votre compte a été créé avec succès sur notre plateforme.

Voici vos identifiants de connexion :

📧 Email : ${userEmail}
🔑 Mot de passe : ${password}

🔗 Lien de connexion : ${loginUrl}

INSTRUCTIONS DE CONNEXION :
1. Cliquez sur le lien ci-dessus ou copiez-le dans votre navigateur
2. Entrez votre email et votre mot de passe
3. Cliquez sur "Connexion"

⚠️ IMPORTANT : Pour des raisons de sécurité, nous vous recommandons de changer votre mot de passe lors de votre première connexion.

Pour changer votre mot de passe :
1. Une fois connecté, cliquez sur votre avatar en haut à droite
2. Cliquez sur "Profil"
3. Cliquez sur "Changer le mot de passe"

Si vous rencontrez des difficultés, n'hésitez pas à nous contacter.

Cordialement,
L'équipe PrestaPro`;

    const mailtoLink = `mailto:${userEmail}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
    window.location.href = mailtoLink;
  }

  // ✅ NOUVEAU : Ouvrir modal mot de passe temporaire
  openTempPasswordModal(user: UserDto) {
    this.tempPasswordUser = user;
    this.newTempPassword = '';
    this.tempPasswordError = '';
    this.showTempPasswordModal = true;
    this.cdr.detectChanges();
  }

  // ✅ NOUVEAU : Fermer modal mot de passe temporaire
  closeTempPasswordModal() {
    this.showTempPasswordModal = false;
    this.tempPasswordUser = null;
    this.newTempPassword = '';
    this.tempPasswordError = '';
    this.cdr.detectChanges();
  }

  // ✅ NOUVEAU : Confirmer et envoyer email avec nouveau mot de passe
  confirmSendTempPassword() {
    // Validation
    if (!this.newTempPassword || this.newTempPassword.trim() === '') {
      this.tempPasswordError = 'Le mot de passe est obligatoire';
      return;
    }

    if (this.newTempPassword.length < 6) {
      this.tempPasswordError = 'Le mot de passe doit contenir au moins 6 caractères';
      return;
    }

    if (!this.tempPasswordUser || !this.tempPasswordUser.id) {
      this.tempPasswordError = 'Utilisateur non trouvé';
      return;
    }

    // ✅ Mettre à jour le mot de passe dans la BDD
    const updatedUser: UserDto = {
      ...this.tempPasswordUser,
      password: this.newTempPassword
    };

    this.authService.tompoPassword(this.currentPassword, this.newTempPassword,this.tempPasswordUser.id).subscribe({
      next: () => {
        this.showSuccessNotification('Mot de passe mis à jour avec succès');
        
        // Envoyer l'email
        this.sendWelcomeEmail(this.tempPasswordUser!.email, this.newTempPassword);
        
        // Fermer le modal
        this.closeTempPasswordModal();
      },
      error: (error) => {
        console.error('❌ Erreur mise à jour mot de passe:', error);
        this.tempPasswordError = error.error?.message || 'Erreur lors de la mise à jour du mot de passe';
        this.cdr.detectChanges();
      }
    });
  }

  confirmDelete(user: UserDto) {
    this.userToDelete = user;
    this.showDeleteConfirm = true;
  }

  cancelDelete() {
    this.showDeleteConfirm = false;
    this.userToDelete = null;
  }

  deleteUser() {
    if (!this.userToDelete || !this.userToDelete.id) {
      return;
    }

    const userId = this.userToDelete.id;
    const userName = `${this.userToDelete.firstName} ${this.userToDelete.lastName}`;

    this.userService.delete(userId).subscribe({
      next: () => {
        this.showSuccessNotification(`Utilisateur "${userName}" supprimé avec succès`);
        this.loadUsers();
        this.cancelDelete();
      },
      error: (error) => {
        this.showErrorNotification(error.error?.message || 'Erreur lors de la suppression');
        console.error('Erreur:', error);
        this.cancelDelete();
      }
    });
  }

  toggleUserActivation(user: UserDto) {
    const action = user.isActive ? 'désactiver' : 'activer';
    const updatedUser = { ...user, isActive: !user.isActive };
    
    this.userService.update(user.id!, updatedUser).subscribe({
      next: () => {
        const message = user.isActive ? 'désactivé' : 'activé';
        this.showSuccessNotification(`Utilisateur ${message} avec succès`);
        this.loadUsers();
      },
      error: (error) => {
        this.showErrorNotification(error.error?.message || `Erreur lors de la ${action}`);
        console.error('Erreur:', error);
      }
    });
  }

  showSuccessNotification(message: string) {
    this.successMessage = message;
    this.errorMessage = '';
    setTimeout(() => {
      this.successMessage = '';
      this.cdr.detectChanges();
    }, 5000);
    this.cdr.detectChanges();
  }

  showErrorNotification(message: string) {
    this.errorMessage = message;
    this.successMessage = '';
    setTimeout(() => {
      this.errorMessage = '';
      this.cdr.detectChanges();
    }, 5000);
    this.cdr.detectChanges();
  }

  get filteredUsers(): UserDto[] {
    let filtered = this.users;

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(user => 
        user.email.toLowerCase().includes(term) ||
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term)
      );
    }

    if (this.filterRole) {
      filtered = filtered.filter(user => user.roleId === this.filterRole);
    }

    if (this.filterStatus !== '') {
      const isActive = this.filterStatus === 'true';
      filtered = filtered.filter(user => user.isActive === isActive);
    }

    return filtered;
  }

  get paginatedUsers(): UserDto[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredUsers.slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredUsers.length / this.itemsPerPage);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  resetFilters() {
    this.searchTerm = '';
    this.filterRole = '';
    this.filterStatus = '';
    this.currentPage = 1;
  }

  exportToCSV() {
    const headers = ['Email', 'Prénom', 'Nom', 'Rôle', 'Statut', 'Prestataire'];
    const data = this.filteredUsers.map(user => {
      const prestataire = this.getPrestataireForUser(user.id!);
      return [
        user.email,
        user.firstName,
        user.lastName,
        user.role?.name || '',
        user.isActive ? 'Actif' : 'Inactif',
        prestataire ? `${prestataire.prenom} ${prestataire.nom}` : ''
      ];
    });

    const csvContent = [
      headers.join(','),
      ...data.map(row => row.join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `utilisateurs_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  get startIndex(): number {
    if (!this.filteredUsers || this.filteredUsers.length === 0) {
      return 0;
    }
    return (this.currentPage - 1) * this.itemsPerPage + 1;
  }
  
  get endIndex(): number {
    if (!this.filteredUsers || this.filteredUsers.length === 0) {
      return 0;
    }
    const end = this.currentPage * this.itemsPerPage;
    return end > this.filteredUsers.length ? this.filteredUsers.length : end;
  }

  getAvatarUrl(imageUrl?: string): string | null {
    if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  getInitials(nom?: string, prenom?: string): string {
    const n = (nom ?? '').charAt(0).toUpperCase();
    const p = (prenom ?? '').charAt(0).toUpperCase();
    return (p + n) || '?';
  }
}