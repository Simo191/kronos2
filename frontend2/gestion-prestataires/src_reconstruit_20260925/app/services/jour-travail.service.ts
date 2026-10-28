import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { CagnotteService } from './cagnotte.service';
import { environment } from '../../environments/environment';

export interface JourTravailDto {
  id?: string;
  prestataireId: string;
  mois: string; // Format: "2025-12"
  nombreJours: number;
  tjm: number;
  description?: string;
  prestataire?: any;
  datetravail?: string;
}

@Injectable({
  providedIn: 'root'
})
export class JourTravailService {
  private apiUrl = `${environment.apiUrl}/jourstravail`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private cagnotteService: CagnotteService
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.token;
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getAll(): Observable<JourTravailDto[]> {
    return this.http.get<JourTravailDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  create(jourTravail: JourTravailDto): Observable<any> {
    return this.http.post(this.apiUrl, jourTravail, { headers: this.getHeaders() }).pipe(
      tap(() => this.cagnotteService.refreshCagnotte())
    );
  }

  update(id: string, jourTravail: JourTravailDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { ...jourTravail, id }, { headers: this.getHeaders() }).pipe(
      tap(() => this.cagnotteService.refreshCagnotte())
    );
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      tap(() => this.cagnotteService.refreshCagnotte())
    );
  }
}