import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface RoleDto {
  id?: string;
  name: string;
  description: string;
  isActive?: boolean;
  userCount?: number;
  createdAt?: Date;
  updatedAt?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/roles`;

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

  getAll(): Observable<RoleDto[]> {
    return this.http.get<RoleDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getById(id: string): Observable<RoleDto> {
    return this.http.get<RoleDto>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(role: RoleDto): Observable<RoleDto> {
    return this.http.post<RoleDto>(this.apiUrl, role, { headers: this.getHeaders() });
  }

  update(id: string, role: RoleDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { ...role, id }, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}