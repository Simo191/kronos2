import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { PrestataireService } from '../../services/prestataire.service';
import { Prestataire } from '../../models/prestataire.model';
import { environment } from '../../../environments/environment';
import { UserService, UserDto } from '../../services/user.service';
import { User } from '../../models/user.model';
import { HasPermissionDirective } from "../../directives/permission.directives";
import { Permission, PermissionService, Role } from '../../services/permission.service';

interface Notification {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  autoClose?: boolean;
}

@Component({
  selector: 'app-profil',
  standalone: true,
  imports: [CommonModule, FormsModule, HasPermissionDirective],
  templateUrl: './profil.component.html',
  styleUrls: ['./profil.component.scss']
})
export class ProfilComponent implements OnInit {
  currentUser: User | null = null;
  prestataire: Prestataire | null = null;
  
  editMode = false;
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  
    //  AJOUT PERMISSIONS
    Permission = Permission;
    Role = Role;
    
  showPasswordModal = false;
  currentPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  
  notifications: Notification[] = [];
  private notificationIdCounter = 0;
  
  successMessage: string = '';
  errorMessage: string = '';
  passwordSuccessMessage: string = '';
  passwordErrorMessage: string = '';

  constructor(
    private authService: AuthService,
    private userService: UserService,    
    private permissionService: PermissionService,
    private prestataireService: PrestataireService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCurrentUser();
    this.loadPrestataire();
    
    // Escape key pour fermer modal
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && this.showPasswordModal) {
        this.forceCloseModal();
      }
    });
  }

  loadCurrentUser(): void {
    this.authService.currentUser.subscribe(user => {
      this.currentUser = user;
      if (user) {
        this.firstName = user.firstName || '';
        this.lastName = user.lastName || '';
        this.email = user.email || '';
      }
    });
  }

  // ✅ CORRECTION NOTIFICATION AUTO-CLOSE
  private addNotification(type: 'success' | 'error' | 'warning' | 'info', message: string, autoClose: boolean = true): void {
    const notification: Notification = {
      id: this.notificationIdCounter++,
      type,
      message,
      autoClose
    };
    
    this.notifications.push(notification);    
    if (autoClose) {
      // ✅ CORRECTION : Capturer l'ID dans une variable
      const notifId = notification.id;      
      setTimeout(() => {
        this.removeNotification(notifId);
        this.cdr.detectChanges();
      }, 5000);
    }
  }
  
  removeNotification(id: number): void {
    const lengthBefore = this.notifications.length;
    this.notifications = this.notifications.filter(n => n.id !== id);
    const lengthAfter = this.notifications.length;
    this.cdr.detectChanges();
  }
  
  showSuccess(message: string, autoClose: boolean = true): void {
    this.addNotification('success', message, autoClose);
  }
  
  showError(message: string, autoClose: boolean = true): void {
    this.addNotification('error', message, autoClose);
  }

  enableEditMode(): void {
    this.editMode = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();
  }

  cancelEdit(): void {
    this.editMode = false;
    if (this.currentUser) {
      this.firstName = this.currentUser.firstName || '';
      this.lastName = this.currentUser.lastName || '';
    }
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();
  }

  // ✅ CORRECTION ENREGISTRER
  saveProfile(): void {    
    const userId = this.currentUser?.id || this.currentUser?.id;
    
    if (!userId) {
      console.error('❌ User ID manquant:', this.currentUser);
      this.errorMessage = 'Utilisateur non connecté';
      this.showError('❌ Utilisateur non connecté');
      return;
    }

    if (!this.firstName.trim() || !this.lastName.trim()) {
      console.error('❌ Nom/prénom manquant');
      this.errorMessage = 'Le nom et le prénom sont obligatoires';
      this.showError('❌ Le nom et le prénom sont obligatoires');
      return;
    }

    // ✅ CORRECTION ROLEID
    const roleId = this.currentUser!.roleId || this.currentUser!.role?.id;
    
    if (!roleId) {
      console.error('❌ RoleId manquant:', this.currentUser);
      this.errorMessage = 'RoleId manquant';
      this.showError('❌ Erreur: RoleId manquant');
      return;
    }

    const updateData: UserDto = {
      id: userId,
      email: this.email,
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      roleId: roleId, // ✅ ROLEIR CORRIGÉ
      isActive: this.currentUser!.isActive !== undefined ? this.currentUser!.isActive : true,
      prestataireId: this.currentUser!.prestataireId || null
    };

    this.userService.update(userId, updateData).subscribe({
      next: () => {        
        // ✅ CORRECTION : Forcer la sortie du mode édition
        this.editMode = false;
        this.successMessage = '✅ Profil mis à jour avec succès !';
        this.errorMessage = '';
        
        this.showSuccess('✅ Profil mis à jour avec succès !');
        
        // Mettre à jour currentUser
        if (this.currentUser) {
          this.currentUser.firstName = this.firstName;
          this.currentUser.lastName = this.lastName;
          this.authService.updateCurrentUser(this.currentUser);
        }
        
        // ✅ FORCE la détection de changement
        this.cdr.detectChanges();
        
        setTimeout(() => {
          this.successMessage = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (err) => {
        console.error('❌ Erreur update profil:', err);
        this.errorMessage = '❌ Erreur lors de la mise à jour du profil';
        this.successMessage = '';
        this.showError('❌ Erreur lors de la mise à jour du profil');
        this.cdr.detectChanges();
      }
    });
  }

  openPasswordModal(): void {
    this.showPasswordModal = true;
    this.currentPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';
    this.passwordSuccessMessage = '';
    this.passwordErrorMessage = '';
    this.cdr.detectChanges();
  }

  forceCloseModal(): void {
    this.showPasswordModal = false;
    this.currentPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';
    this.passwordSuccessMessage = '';
    this.passwordErrorMessage = '';
    this.cdr.detectChanges();
  }

  closePasswordModal(): void {
    this.forceCloseModal();
  }

  changePassword(): void {
    this.passwordErrorMessage = '';
    this.passwordSuccessMessage = '';

    if (!this.currentPassword || !this.newPassword || !this.confirmPassword) {
      this.passwordErrorMessage = 'Tous les champs sont obligatoires';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.passwordErrorMessage = 'Les mots de passe ne correspondent pas';
      return;
    }

    if (this.newPassword.length < 6) {
      this.passwordErrorMessage = 'Le mot de passe doit contenir au moins 6 caractères';
      return;
    }

    if (this.newPassword === this.currentPassword) {
      this.passwordErrorMessage = 'Le nouveau mot de passe doit être différent de l\'ancien';
      return;
    }

    this.authService.changePassword(this.currentPassword, this.newPassword,null!).subscribe({
      next: () => {
        this.passwordSuccessMessage = '✅ Mot de passe modifié avec succès !';
        this.passwordErrorMessage = '';
        this.showSuccess('✅ Mot de passe modifié avec succès !');
        
        setTimeout(() => {
          this.forceCloseModal();
        }, 2000);
      },
      error: (err) => {
        console.error('❌ Erreur changement password:', err);
        if (err.status === 401 || err.error?.message?.includes('incorrect')) {
          this.passwordErrorMessage = '❌ Mot de passe actuel incorrect';
          this.showError('❌ Mot de passe actuel incorrect');
        } else {
          this.passwordErrorMessage = '❌ Erreur lors du changement de mot de passe';
          this.showError('❌ Erreur lors du changement de mot de passe');
        }
        this.passwordSuccessMessage = '';
      }
    });
  }

  loadPrestataire(): void {
    this.prestataireService.getAll().subscribe({
      next: (list) => {
        const userId = this.currentUser?.id;
        if (userId) {
          this.prestataire = list.find(p => p.userId === userId) ?? null;
          this.cdr.detectChanges();
        }
      }
    });
  }

  getAvatarUrl(imageUrl?: string): string | null {
    if (!imageUrl) return null;
    if (imageUrl.startsWith('http')) return imageUrl;
    return `${environment.apiUrl.replace('/api', '')}/${imageUrl}`;
  }

  getInitials(): string {
    if (!this.currentUser) return '?';
    const firstInitial = this.currentUser.firstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = this.currentUser.lastName?.charAt(0)?.toUpperCase() || '';
    return `${firstInitial}${lastInitial}`;
  }

  getRoleName(): string {
    if (!this.currentUser?.role) return 'Utilisateur';
    return this.currentUser.role.name || 'Utilisateur';
  }
}