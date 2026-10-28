import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface MoisVerrouilleDto {
  id?: string;
  mois: string; // Format: "2025-12"
  verrouilleParUserId?: string;
  verrouilleParUserName?: string;
  dateVerrouillage?: string;
  commentaire?: string;
}

@Injectable({
  providedIn: 'root'
})
export class MoisVerrouilleService {
  private apiUrl = `${environment.apiUrl}/moisverrouilles`;
  
  // ✅ Observable pour les mois verrouillés
  private moisVerrouillesSubject = new BehaviorSubject<MoisVerrouilleDto[]>([]);
  public moisVerrouilles$ = this.moisVerrouillesSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    // Charger les mois verrouillés au démarrage
    this.loadMoisVerrouilles();
  }

  private getHeaders(): HttpHeaders {
    const token = this.authService.token;
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  loadMoisVerrouilles(): void {
    this.getAll().subscribe({
      next: (data) => {
        this.moisVerrouillesSubject.next(data);
      },
      error: (err) => {
        console.error('Erreur chargement mois verrouillés:', err);
        this.moisVerrouillesSubject.next([]);
      }
    });
  }

  getAll(): Observable<MoisVerrouilleDto[]> {
    return this.http.get<MoisVerrouilleDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  verrouiller(mois: string, commentaire?: string): Observable<any> {
    return this.http.post(this.apiUrl, { mois, commentaire }, { headers: this.getHeaders() }).pipe(
      tap(() => this.loadMoisVerrouilles())
    );
  }

  deverrouiller(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      tap(() => this.loadMoisVerrouilles())
    );
  }

  // ✅ Vérifier si un mois est verrouillé (méthode synchrone)
  isMoisVerrouille(mois: string): boolean {
    const moisVerrouilles = this.moisVerrouillesSubject.value;
    return moisVerrouilles.some(m => m.mois === mois);
  }

  // ✅ Récupérer l'objet complet d'un mois verrouillé
  getMoisVerrouille(mois: string): MoisVerrouilleDto | undefined {
    const moisVerrouilles = this.moisVerrouillesSubject.value;
    return moisVerrouilles.find(m => m.mois === mois);
  }
}