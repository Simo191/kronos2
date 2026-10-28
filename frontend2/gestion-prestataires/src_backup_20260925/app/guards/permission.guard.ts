import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { PermissionService, Permission } from '../services/permission.service';

/**
 * Guard de permissions
 *
 * ✅ SÉCURISÉ : avant de vérifier la permission, on relit l'utilisateur
 * depuis le JWT (getUserFromToken) pour s'assurer que le rôle utilisé
 * est bien celui signé par le serveur, et non une valeur manipulée.
 *
 * Double vérification :
 *  1. Le token est valide et non expiré
 *  2. L'utilisateur possède la permission requise
 */
export const permissionGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService      = inject(AuthService);
  const permissionService = inject(PermissionService);
  const router           = inject(Router);

  // ✅ ÉTAPE 1 : Relire l'utilisateur depuis le JWT, pas depuis le BehaviorSubject
  // qui pourrait avoir été alimenté avec une valeur en cache
  const userFromToken = authService.getUserFromToken();

  if (!userFromToken) {
    // Token absent ou expiré
    router.navigate(['/login']);
    return false;
  }

  // ✅ ÉTAPE 2 : Vérifier la permission requise par la route
  const requiredPermission = route.data['permission'] as Permission;

  if (!requiredPermission) {
    // Pas de permission requise → accès libre (authentification suffit)
    return true;
  }

  if (permissionService.hasPermission(requiredPermission)) {
    return true;
  }

  // Permission manquante
  console.warn(`❌ Accès refusé : permission "${requiredPermission}" requise (rôle actuel : ${userFromToken.role?.name})`);
  router.navigate(['/unauthorized']);
  return false;
};