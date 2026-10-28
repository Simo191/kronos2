import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Entreprise, CreateEntrepriseDto, UpdateEntrepriseDto } from '../models/entreprise.model';

@Injectable({ providedIn: 'root' })
export class EntrepriseService {
  private apiUrl = `${environment.apiUrl}/entreprises`;

  constructor(private http: HttpClient) {}

  /** Liste des entreprises (actives par défaut). */
  getAll(includeInactive: boolean = false): Observable<Entreprise[]> {
    const qs = includeInactive ? '?includeInactive=true' : '';
    return this.http.get<Entreprise[]>(`${this.apiUrl}${qs}`);
  }

  getById(id: string): Observable<Entreprise> {
    return this.http.get<Entreprise>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateEntrepriseDto): Observable<Entreprise> {
    return this.http.post<Entreprise>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateEntrepriseDto): Observable<Entreprise> {
    return this.http.put<Entreprise>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
