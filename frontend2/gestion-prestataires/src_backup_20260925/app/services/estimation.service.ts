import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { Estimation, CreateEstimationCommand, UpdateEstimationCommand } from '../models/estimation.model';

@Injectable({
  providedIn: 'root'
})
export class EstimationService {
  private apiUrl = `${environment.apiUrl}/estimations`;

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

  getAll(): Observable<Estimation[]> {
    return this.http.get<Estimation[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getById(id: string): Observable<Estimation> {
    return this.http.get<Estimation>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(command: CreateEstimationCommand): Observable<any> {
    return this.http.post(this.apiUrl, command, { headers: this.getHeaders() });
  }

  update(id: string, command: UpdateEstimationCommand): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, command, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}