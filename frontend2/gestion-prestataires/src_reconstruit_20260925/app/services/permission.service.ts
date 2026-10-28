import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable, map } from 'rxjs';

export enum Role {
  Admin = 'Admin',
  Manager = 'Manager',
  User = 'User'
}

export enum Permission {
  // Modules
  VIEW_PRESTATAIRES = 'VIEW_PRESTATAIRES',
  MANAGE_PRESTATAIRES = 'MANAGE_PRESTATAIRES',
  VIEW_CONTRATS = 'VIEW_CONTRATS',
  MANAGE_CONTRATS = 'MANAGE_CONTRATS',
  VIEW_JOURS_TRAVAIL = 'VIEW_JOURS_TRAVAIL',
  MANAGE_JOURS_TRAVAIL = 'MANAGE_JOURS_TRAVAIL',
  VIEW_PLANNING = 'VIEW_PLANNING',
  MANAGE_PLANNING = 'MANAGE_PLANNING',
  VIEW_TYPE_CONTRAT = 'VIEW_TYPE_CONTRAT',
  MANAGE_TYPE_CONTRAT = 'MANAGE_TYPE_CONTRAT',
  VIEW_TYPE_PLANNING = 'VIEW_TYPE_PLANNING',
  MANAGE_TYPE_PLANNING = 'MANAGE_TYPE_PLANNING',
  VIEW_CAGNOTTES = 'VIEW_CAGNOTTES',
  MANAGE_CAGNOTTES = 'MANAGE_CAGNOTTES',
  VIEW_USERS = 'VIEW_USERS',
  MANAGE_USERS = 'MANAGE_USERS',
  MANAGE_USER_ROLES = 'MANAGE_USER_ROLES',
  VIEW_ROLES = 'VIEW_ROLES',
  MANAGE_ROLES = 'MANAGE_ROLES',
  
  // Données sensibles
  VIEW_TJM = 'VIEW_TJM',
  VIEW_REVENUE = 'VIEW_REVENUE',
  VIEW_TAUX_JOURNALIER = 'VIEW_TAUX_JOURNALIER',

  //   Entreprises (SSII de rattachement) — paramétrage réservé Admin/Manager
  VIEW_ENTREPRISES = 'VIEW_ENTREPRISES',
  MANAGE_ENTREPRISES = 'MANAGE_ENTREPRISES',
}

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  
  // ✅ Matrice de permissions par rôle
  private readonly rolePermissions: Record<Role, Permission[]> = {
    [Role.Admin]: [
      // Admin a TOUTES les permissions
      Permission.VIEW_PRESTATAIRES,
      Permission.MANAGE_PRESTATAIRES,
      Permission.VIEW_CONTRATS,
      Permission.MANAGE_CONTRATS,
      Permission.VIEW_JOURS_TRAVAIL,
      Permission.MANAGE_JOURS_TRAVAIL,
      Permission.VIEW_PLANNING,
      Permission.MANAGE_PLANNING,
      Permission.VIEW_TYPE_CONTRAT,
      Permission.MANAGE_TYPE_CONTRAT,
      Permission.VIEW_TYPE_PLANNING,
      Permission.MANAGE_TYPE_PLANNING,
      Permission.VIEW_CAGNOTTES,
      Permission.MANAGE_CAGNOTTES,
      Permission.VIEW_USERS,
      Permission.MANAGE_USERS,
      Permission.MANAGE_USER_ROLES,
      Permission.VIEW_ROLES,
      Permission.MANAGE_ROLES,
      Permission.VIEW_TJM,
      Permission.VIEW_REVENUE,
      Permission.VIEW_TAUX_JOURNALIER,
      Permission.VIEW_ENTREPRISES,
      Permission.MANAGE_ENTREPRISES,
    ],

    [Role.Manager]: [
      // Manager : Tout SAUF Type Contrat, Type Planning, Cagnottes, Rôles
      Permission.VIEW_PRESTATAIRES,
      Permission.MANAGE_PRESTATAIRES,
      Permission.VIEW_CONTRATS,
      Permission.MANAGE_CONTRATS,
      Permission.VIEW_JOURS_TRAVAIL,
      Permission.MANAGE_JOURS_TRAVAIL,
      Permission.VIEW_PLANNING,
      Permission.MANAGE_PLANNING,
      Permission.VIEW_USERS,
      Permission.MANAGE_USERS,
      // ❌ PAS MANAGE_USER_ROLES (ne peut pas choisir le rôle)
      Permission.VIEW_TJM,
      Permission.VIEW_REVENUE,
      Permission.VIEW_TAUX_JOURNALIER,
      Permission.VIEW_ENTREPRISES,
      Permission.MANAGE_ENTREPRISES,
    ],

    [Role.User]: [
      // User : Très restreint
      Permission.VIEW_PRESTATAIRES,    //   Lecture seule, et SEULEMENT sa propre fiche (backend filtre)
      // ❌ PAS MANAGE_PRESTATAIRES : pas d'édition, ni d'ajout, ni de désactivation
      Permission.VIEW_CONTRATS,        // Seulement ses contrats
      Permission.VIEW_JOURS_TRAVAIL,   // Seulement ses jours
      //Permission.MANAGE_JOURS_TRAVAIL, // Peut ajouter ses jours
      Permission.VIEW_PLANNING,        // Voir tout le planning
      Permission.MANAGE_PLANNING,      // Gérer SES shifts uniquement
      // ❌ PAS VIEW_TJM, VIEW_REVENUE, VIEW_TAUX_JOURNALIER
    ]
  };

  constructor(private authService: AuthService) {}

  /**
   * ✅ Vérifier si l'utilisateur actuel a une permission
   */
  hasPermission(permission: Permission): boolean {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser || !currentUser.role) {
      return false;
    }

    const userRole = currentUser.role.name as Role;
    const permissions = this.rolePermissions[userRole] || [];
    
    return permissions.includes(permission);
  }

  /**
   * ✅ Vérifier plusieurs permissions (ET logique)
   */
  hasAllPermissions(...permissions: Permission[]): boolean {
    return permissions.every(p => this.hasPermission(p));
  }

  /**
   * ✅ Vérifier au moins une permission (OU logique)
   */
  hasAnyPermission(...permissions: Permission[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  /**
   * ✅ Vérifier si l'utilisateur a un rôle spécifique
   */
  hasRole(role: Role): boolean {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser || !currentUser.role) {
      return false;
    }
    return currentUser.role.name === role;
  }

  /**
   * ✅ Obtenir le rôle de l'utilisateur actuel
   */
  getCurrentRole(): Role | null {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser || !currentUser.role) {
      return null;
    }
    return currentUser.role.name as Role;
  }

  /**
   * ✅ Vérifier si user peut voir un contrat spécifique
   */
  canViewContrat(contratPrestataireId: string): boolean {
    if (this.hasRole(Role.Admin) || this.hasRole(Role.Manager)) {
      return true; // Admin et Manager voient tous les contrats
    }

    // User : seulement ses contrats (via prestataire lié)
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) return false;

    // TODO: Comparer avec le prestataireId du user connecté
    // Pour l'instant, on retourne true si c'est un User
    return this.hasRole(Role.User);
  }

  /**
   * ✅ Vérifier si user peut modifier un contrat
   */
  canManageContrat(contratPrestataireId: string): boolean {
    if (this.hasRole(Role.User)) {
      return false; // User ne peut JAMAIS modifier un contrat
    }
    return this.hasPermission(Permission.MANAGE_CONTRATS);
  }

  /**
   * ✅ Vérifier si user peut voir un jour de travail
   */
  canViewJourTravail(jourTravailPrestataireId: string): boolean {
    if (this.hasRole(Role.Admin) || this.hasRole(Role.Manager)) {
      return true; // Admin et Manager voient tous les jours
    }

    // User : seulement ses jours
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) return false;

    // TODO: Comparer avec le prestataireId du user
    return this.hasRole(Role.User);
  }

  /**
   * ✅ Vérifier si user peut gérer un shift
   */
  canManageShift(shiftPrestataireId: string): boolean {
    if (this.hasRole(Role.Admin) || this.hasRole(Role.Manager)) {
      return true; // Admin et Manager gèrent tous les shifts
    }

    // User : seulement SES shifts
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) return false;

    // TODO: Comparer avec le prestataireId du user
    return this.hasRole(Role.User);
  }

  /**
   * ✅ Obtenir l'ID du prestataire lié au user actuel
   */
  getCurrentUserPrestataireId(): string | null {
    const currentUser = this.authService.currentUserValue;
    // TODO: Récupérer depuis le backend ou depuis le user
    return null; // À implémenter
  }

  /**
   * ✅ Vérifier si user peut gérer les rôles des autres users
   */
  canManageUserRoles(): boolean {
    return this.hasPermission(Permission.MANAGE_USER_ROLES);
  }

  /**
   * ✅ Obtenir le rôle par défaut pour la création d'un user
   */
  getDefaultRoleForUserCreation(): Role {
    if (this.hasRole(Role.Admin)) {
      return Role.User; // Admin peut choisir, mais par défaut User
    }
    
    if (this.hasRole(Role.Manager)) {
      return Role.User; // Manager crée toujours des Users
    }
    
    return Role.User;
  }
}