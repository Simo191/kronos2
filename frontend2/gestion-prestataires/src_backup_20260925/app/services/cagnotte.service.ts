import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PermissionService, Permission } from './permission.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Cagnotte, SourceBudget } from '../models/cagnotte.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CagnotteService {
  private apiUrl = `${environment.apiUrl}/cagnottes`;

  private cagnottesSubject = new BehaviorSubject<Cagnotte[]>([]);
  private selectedCagnotteSubject = new BehaviorSubject<Cagnotte | null>(null);
  private cagnotteActuelleSubject = new BehaviorSubject<Cagnotte | null>(null);

  cagnottes$ = this.cagnottesSubject.asObservable();
  selectedCagnotte$ = this.selectedCagnotteSubject.asObservable();
  cagnotteActuelle$ = this.cagnotteActuelleSubject.asObservable();

  constructor(private http: HttpClient, private permissionService: PermissionService) {
    // ✅ Ne charger que si l'utilisateur a la permission — évite les 403 en console
    if (this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES)) {
      this.loadAllCagnottes();
      this.loadCagnotteActuelle();
    }
  }

  private loadAllCagnottes(): void {
    this.getAll().subscribe({
      next: (cagnottes) => {
        this.cagnottesSubject.next(cagnottes);
        
        // ✅ NOUVEAU : Re-sélectionner la cagnotte actuelle pour mettre à jour les valeurs
        const currentSelected = this.selectedCagnotteSubject.value;
        if (currentSelected && currentSelected.annee) {
          const updatedCagnotte = cagnottes.find(c => c.annee === currentSelected.annee);
          if (updatedCagnotte) {         
            this.selectedCagnotteSubject.next(updatedCagnotte);
          }
        } else if (!currentSelected && cagnottes.length > 0) {
          const actuelle = cagnottes.find(c => c.annee === new Date().getFullYear());
          this.selectedCagnotteSubject.next(actuelle || cagnottes[0]);
        }
      },
      error: (err) => {
        //console.error('❌ Erreur chargement cagnottes:', err);
        this.cagnottesSubject.next([]);
      }
    });
  }

  private loadCagnotteActuelle(): void {
    this.getCagnotteActuelle().subscribe({
      next: (cagnotte) => {
        this.cagnotteActuelleSubject.next(cagnotte);
      },
      error: (err) => {
        //console.error('❌ Erreur chargement cagnotte actuelle:', err);
        this.cagnotteActuelleSubject.next(null);
      }
    });
  }

  getAll(): Observable<Cagnotte[]> {
    return this.http.get<Cagnotte[]>(this.apiUrl);
  }

  getCagnotteActuelle(): Observable<Cagnotte> {
    return this.http.get<Cagnotte>(`${this.apiUrl}/actuelle`).pipe(
      tap(c => this.cagnotteActuelleSubject.next(c))
    );
  }

  getByAnnee(annee: number): Observable<Cagnotte> {
    return this.http.get<Cagnotte>(`${this.apiUrl}/annee/${annee}`);
  }

  create(cagnotte: Partial<Cagnotte>): Observable<Cagnotte> {
    const payload = {
      annee: cagnotte.annee,
      sourcesBudget: cagnotte.sourcesBudget?.map(s => ({
        nom: s.nom,
        code: s.code,
        montant: s.montant,
        description: s.description
      })) || []
    };

    return this.http.post<Cagnotte>(this.apiUrl, payload).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  update(id: string, cagnotte: Partial<Cagnotte>): Observable<void> {
    const payload = {
      sourcesBudget: cagnotte.sourcesBudget?.map(s => ({
        id: s.id || null,
        nom: s.nom,
        code: s.code,
        montant: s.montant,
        description: s.description
      })) || []
    };

    return this.http.put<void>(`${this.apiUrl}/${id}`, payload).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  recalculer(annee: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/recalculer/${annee}`, {}).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  // Sources de budget
  getSourcesBudget(cagnotteId: string): Observable<SourceBudget[]> {
    return this.http.get<SourceBudget[]>(`${this.apiUrl}/${cagnotteId}/sources`);
  }

  addSourceBudget(cagnotteId: string, source: Partial<SourceBudget>): Observable<SourceBudget> {
    return this.http.post<SourceBudget>(`${this.apiUrl}/${cagnotteId}/sources`, source).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  updateSourceBudget(sourceId: string, source: Partial<SourceBudget>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/sources/${sourceId}`, source).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  deleteSourceBudget(sourceId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/sources/${sourceId}`).pipe(
      tap(() => {
        this.loadAllCagnottes();
        this.loadCagnotteActuelle();
      })
    );
  }

  selectYear(annee: number): void {
    const cagnotte = this.cagnottesSubject.value.find(c => c.annee === annee);
    if (cagnotte) {
      this.selectedCagnotteSubject.next(cagnotte);
    }
  }

  // ✅ MODIFIÉ : Cette méthode est appelée depuis jour-travail.service.ts
  refreshCagnotte(): void {
    if (!this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES)) return;
    this.loadCagnotteActuelle();
    this.loadAllCagnottes();
  }

  refresh(): void {
    if (!this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES)) return;
    this.loadAllCagnottes();
    this.loadCagnotteActuelle();
  }
}