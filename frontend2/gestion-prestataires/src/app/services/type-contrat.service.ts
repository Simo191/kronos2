import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface TypeContratDto {
  id?: string;
  libelle: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TypeContratService {
  private apiUrl = `${environment.apiUrl}/typescontrat`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.token;
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getAll(): Observable<TypeContratDto[]> {
    return this.http.get<TypeContratDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  create(type: TypeContratDto): Observable<any> {
    return this.http.post(this.apiUrl, type, { headers: this.getHeaders() });
  }

  update(id: string, type: TypeContratDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { ...type, id }, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
