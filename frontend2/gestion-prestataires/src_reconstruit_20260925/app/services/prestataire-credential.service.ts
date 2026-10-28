import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  PrestataireCredential,
  CreatePrestataireCredentialDto,
  UpdatePrestataireCredentialDto,
  RevealCredentialResponse
} from '../models/prestataire-credential.model';

@Injectable({ providedIn: 'root' })
export class PrestataireCredentialService {
  private base(prestataireId: string): string {
    return `${environment.apiUrl}/prestataires/${prestataireId}/credentials`;
  }

  //   Liste des comptes (mdp masqué)
  getAll(prestataireId: string): Observable<PrestataireCredential[]> {
    return this.http.get<PrestataireCredential[]>(this.base(prestataireId));
  }

  create(prestataireId: string, dto: CreatePrestataireCredentialDto): Observable<PrestataireCredential> {
    return this.http.post<PrestataireCredential>(this.base(prestataireId), dto);
  }

  update(prestataireId: string, credentialId: string, dto: UpdatePrestataireCredentialDto): Observable<PrestataireCredential> {
    return this.http.put<PrestataireCredential>(`${this.base(prestataireId)}/${credentialId}`, dto);
  }

  delete(prestataireId: string, credentialId: string): Observable<void> {
    return this.http.delete<void>(`${this.base(prestataireId)}/${credentialId}`);
  }

  //   Demande le mdp en clair — backend exige le mdp utilisateur courant
  reveal(prestataireId: string, credentialId: string, userPassword: string): Observable<RevealCredentialResponse> {
    return this.http.post<RevealCredentialResponse>(
      `${this.base(prestataireId)}/${credentialId}/reveal`,
      { userPassword }
    );
  }

  constructor(private http: HttpClient) {}
}
