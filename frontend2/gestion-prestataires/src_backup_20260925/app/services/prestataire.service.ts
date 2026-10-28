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

  getAll(): Observable<Prestataire[]> {
    return this.http.get<Prestataire[]>(this.apiUrl);
  }

  getPrestataireShift(): Observable<Prestataire[]> {
    
    return this.http.get<Prestataire[]>(`${this.apiUrl}/prestataireshift`);
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