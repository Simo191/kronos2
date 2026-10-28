import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import {
  ApplicationPlanning,
  CreateApplicationPlanningDto,
  UpdateApplicationPlanningDto
} from '../models/planning.model';

@Injectable({ providedIn: 'root' })
export class PlanningService {
  private apiUrl = `${environment.apiUrl}/applicationsplanning`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.authService.token}`
    });
  }

  getAll(): Observable<ApplicationPlanning[]> {
    return this.http.get<ApplicationPlanning[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getById(id: string): Observable<ApplicationPlanning> {
    return this.http.get<ApplicationPlanning>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(dto: CreateApplicationPlanningDto): Observable<ApplicationPlanning> {
    return this.http.post<ApplicationPlanning>(this.apiUrl, dto, { headers: this.getHeaders() });
  }

  update(id: string, dto: UpdateApplicationPlanningDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}