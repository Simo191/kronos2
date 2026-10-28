import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard d'authentification
 *
 * ✅ SÉCURISÉ : utilise isAuthenticated() qui vérifie
 * la présence ET l'expiration du JWT, pas juste le localStorage.
 */
export const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Token absent ou expiré → login
  router.navigate(['/login']);
  return false;
};