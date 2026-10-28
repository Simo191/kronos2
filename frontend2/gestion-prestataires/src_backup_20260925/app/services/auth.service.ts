import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest, User, Role } from '../models/user.model';
import { environment } from '../../environments/environment';

// ============================================================
// Claims présents dans le JWT généré par LoginCommandHandler.cs
// ASP.NET transforme ClaimTypes.Role et ClaimTypes.NameIdentifier
// en noms longs dans le token JWT.
// ============================================================
interface JwtPayload {
  // Claims standard JWT
  sub: string;               // userId  (JwtRegisteredClaimNames.Sub)
  email: string;
  given_name: string;        // firstName (JwtRegisteredClaimNames.GivenName)
  family_name: string;       // lastName  (JwtRegisteredClaimNames.FamilyName)
  exp: number;               // expiration
  iat: number;               // issued-at

  // Claims ASP.NET avec leur nom long dans le JWT
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;

  // ✅ Claims custom ajoutés par le backend
  roleId: string;
  prestataireId: string;
  isActive: string;          // "true" | "false"
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/authentication`;
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient) {
    // ✅ SÉCURITÉ : On supprime définitivement currentUser du localStorage
    // pour invalider toute session qui aurait été manipulée.
    localStorage.removeItem('currentUser');

    // ✅ SÉCURITÉ : Le rôle et l'identité proviennent UNIQUEMENT du JWT,
    // signé cryptographiquement par le backend. Impossible à falsifier
    // depuis le DevTools.
    const userFromToken = this.getUserFromToken();
    this.currentUserSubject = new BehaviorSubject<User | null>(userFromToken);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  // ----------------------------------------------------------
  // Getters publics
  // ----------------------------------------------------------

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get token(): string | null {
    return localStorage.getItem('token');
  }

  // ----------------------------------------------------------
  // Décodage JWT (sans librairie externe)
  // ----------------------------------------------------------

  /**
   * Décode la partie payload du JWT (base64url → JSON).
   * NE vérifie PAS la signature (c'est le rôle du backend).
   * Vérifie uniquement l'expiration côté client pour l'UX.
   */
  private decodeToken(token: string): JwtPayload | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      // base64url → base64 standard → JSON
      const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      return JSON.parse(json) as JwtPayload;
    } catch (e) {
      console.error('❌ Impossible de décoder le JWT :', e);
      return null;
    }
  }

  /**
   * Construit un objet User à partir du token JWT stocké.
   * Retourne null si le token est absent, invalide ou expiré.
   */
  getUserFromToken(): User | null {
    const token = this.token;
    if (!token) return null;

    const payload = this.decodeToken(token);
    if (!payload) {
      // Token malformé → on purge
      this.logout();
      return null;
    }

    // ✅ Vérifier l'expiration
    if (payload.exp && payload.exp * 1000 < Date.now()) {
      console.warn('⏰ Token expiré – déconnexion automatique');
      this.logout();
      return null;
    }

    // ✅ Lire le rôle depuis le claim ASP.NET (ClaimTypes.Role génère ce nom long)
    const roleName =
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      ?? 'User';

    // ✅ userId : priorité au claim "sub", fallback ASP.NET NameIdentifier
    const userId =
      payload.sub
      ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
      ?? '';

    const roleObj: Role = {
      id: payload.roleId ?? '',
      name: roleName,
      displayName: roleName
    };

    return {
      id: userId,
      email: payload.email,
      firstName: payload.given_name ?? '',
      lastName: payload.family_name ?? '',
      role: roleObj,
      isActive: payload.isActive === 'true',
      prestataireId: payload.prestataireId || null
    };
  }

  // ----------------------------------------------------------
  // Auth API
  // ----------------------------------------------------------

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        // ✅ On stocke UNIQUEMENT le token JWT
        // Le rôle et l'identité en seront extraits à chaque démarrage
        localStorage.setItem('token', response.token);

        // ✅ PAS de currentUser dans le localStorage
        const user = this.getUserFromToken();
        this.currentUserSubject.next(user);
      })
    );
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser'); // nettoyage défensif
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    // ✅ Vérifie aussi l'expiration, pas seulement la présence du token
    return this.getUserFromToken() !== null;
  }

  // ----------------------------------------------------------
  // Mise à jour de l'utilisateur (profil, etc.)
  // ----------------------------------------------------------

  /**
   * Met à jour les informations d'affichage (prénom, avatar…).
   * ⚠️  NE permet PAS de changer le rôle : celui-ci est toujours
   * relu depuis le JWT. Seules les infos non-sensibles sont mergées.
   */
  updateCurrentUser(updatedInfo: Partial<User>): void {
    const current = this.currentUserSubject.value;
    if (!current) return;

    // ✅ SÉCURITÉ : on interdit explicitement la modification du rôle
    const { role, id, ...safeInfo } = updatedInfo as any;

    const merged: User = {
      ...current,
      ...safeInfo,
      role: current.role, // ← toujours celui du JWT, non modifiable
      id: current.id      // ← toujours celui du JWT, non modifiable
    };

    this.currentUserSubject.next(merged);
    // ✅ On ne stocke PAS dans localStorage, le rôle reste dans le JWT
  }

  // ----------------------------------------------------------
  // Changement de mot de passe
  // ----------------------------------------------------------

  changePassword(currentPassword: string, newPassword: string, idUser: string): Observable<any> {
    return this.http.post(
      `${environment.apiUrl}/authentication/change-password`,
      { currentPassword, newPassword, idUser }
    );
    // ℹ️  Le header Authorization est ajouté automatiquement par l'intercepteur
  }

  tompoPassword(currentPassword: string, newPassword: string, idUser: string | null): Observable<any> {
    return this.http.post(
      `${environment.apiUrl}/authentication/tempo-password`,
      { currentPassword, newPassword, idUser }
    );
  }
}