import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface UserDto {
  id?: string;
  email: string;
  password?: string;
  firstName: string;
  lastName: string;
  roleId: string | null; 
  isActive: boolean;
  prestataireId?: string | null; // ✅ Prestataire lié (calculé côté frontend)
  role?: { id: string; name: string };
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

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

  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getById(id: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(user: UserDto): Observable<any> {
    return this.http.post(this.apiUrl, user, { headers: this.getHeaders() });
  }

  update(id: string, user: UserDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { ...user, id }, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
  
}
