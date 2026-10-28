import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Prestataire } from '../models/prestataire.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PrestataireService {
  private apiUrl = `${environment.apiUrl}/prestataires`;

  constructor(private http: HttpClient) { }

  //   Par défaut : actifs uniquement. Passer true pour inclure les désactivés
  //   (historique). Réservé Admin/Manager côté backend.
  getAll(includeInactive: boolean = false): Observable<Prestataire[]> {
    const qs = includeInactive ? '?includeInactive=true' : '';
    return this.http.get<Prestataire[]>(`${this.apiUrl}${qs}`);
  }

  //   Alias historique conservé pour ne pas casser les appels existants.
  getAllIncludingInactive(): Observable<Prestataire[]> {
    return this.getAll(true);
  }

  getPrestataireShift(includeInactive: boolean = false): Observable<Prestataire[]> {
    const qs = includeInactive ? '?includeInactive=true' : '';
    return this.http.get<Prestataire[]>(`${this.apiUrl}/prestataireshift${qs}`);
  }

  //   Active / désactive un compte prestataire (toggle).
  //   Bloque aussi la connexion du User lié si présent.
  //   reason : motif facultatif, utilisé uniquement lors d'une désactivation.
  toggleActive(id: string, reason?: string | null): Observable<{ id: string; isActive: boolean }> {
    return this.http.patch<{ id: string; isActive: boolean }>(
      `${this.apiUrl}/${id}/toggle-active`,
      { reason: reason ?? null }
    );
  }

  getById(id: string): Observable<Prestataire> {
    return this.http.get<Prestataire>(`${this.apiUrl}/${id}`);
  }

  create(prestataire: Partial<Prestataire>): Observable<Prestataire> {
    return this.http.post<Prestataire>(this.apiUrl, prestataire);
  }

  update(id: string, prestataire: Partial<Prestataire>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, prestataire);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // ✅ NOUVEAU : Upload image avatar (stockée wwwroot/images/prestataires/{id}.ext)
  uploadImage(id: string, file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(
      `${this.apiUrl}/${id}/upload-image`,
      formData
    );
  }

  // ✅ NOUVEAU : Supprimer l'image d'un prestataire
  deleteImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/image`);
  }
}