import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Intercepteur HTTP
 *
 * ✅ Ajoute le JWT à chaque requête sortante
 * ✅ Gère automatiquement les 401 (token expiré côté serveur)
 *    → déconnexion + redirection vers /login
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  // Cloner la requête avec le header Authorization si token présent
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // ✅ SÉCURITÉ : le serveur rejette le token (expiré, falsifié…)
        // On force la déconnexion immédiatement
        console.warn('🔒 Token rejeté par le serveur (401) – déconnexion forcée');

        // Injection lazy pour éviter la dépendance circulaire
        const authService = inject(AuthService);
        const router = inject(Router);

        authService.logout();
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};