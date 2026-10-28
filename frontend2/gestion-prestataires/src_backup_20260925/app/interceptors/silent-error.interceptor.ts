// interceptors/silent-error.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

/**
 * Intercepteur qui filtre les erreurs HTTP attendues pour ne pas
 * polluer la console avec des erreurs 400/401/403 normales
 * (ex: user sans permission sur certaines APIs)
 */
export const silentErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      // ─── Erreurs silencieuses (attendues selon le rôle) ─────────────────
      const silentUrls = [
        '/api/cagnottes/actuelle',   // User n'a pas accès → 403 normal
        '/api/authentication/login', // Mauvais identifiants → 400 normal
      ];

      const isSilentUrl = silentUrls.some(url => req.url.includes(url));
      const isSilentStatus = [400, 401, 403, 404].includes(error.status);

      if (isSilentUrl && isSilentStatus) {
        // Erreur attendue → retransmettre sans logger
        return throwError(() => error);
      }

      // ─── Erreurs réseau / serveur → log minimal ──────────────────────────
      if (error.status === 0) {
        // Connexion perdue — seulement un warning
        console.warn('[Kronos] Connexion au serveur perdue');
      } else if (error.status >= 500) {
        // Erreur serveur → log utile
        console.error(`[Kronos] Erreur serveur ${error.status} sur ${req.url}`);
      }
      // 400/401/403/404 sur d'autres URLs → pas de log non plus

      return throwError(() => error);
    })
  );
};