import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface ContratDto {
  id?: string;
  prestataireId: string;
  typeContratId: string;

  dateDebut: string;
  dateFin?: string;

  seuilJoursMois?: number | null;

  nombreJours: number;
  tjm: number;
  montantTotal?: number;

  joursUtilises?: number;
  documentUrl: string;

  // ✅ NOUVEAU : Activation manuelle du contrat
  active?: boolean;

  //   Suivi des commandes
  numeroCommande?: string | null;
  referenceCtr?: string | null;
  tranche?: TrancheContrat;
  applicationId?: string | null;
  applicationNom?: string | null;
  commentaire?: string | null;
  //   Répartition enregistrée (vide = proposition automatique)
  repartitions?: RepartitionAnnuelle[];

  prestataire?: any;
  typeContrat?: any;
}

export type TrancheContrat = 'Ferme' | 'Option';

export interface RepartitionAnnuelle {
  annee: number;
  nombreJours: number;
}

//   ─── Suivi des commandes (GET /contrats/suivi-commandes) ───
export interface SuiviAnnee {
  annee: number;
  joursPrevus: number;
  montantPrevu: number;
  joursConsommes: number;
  montantConsomme: number;
}

export type SourceLogiciel = 'contrat' | 'planning' | 'aucun';

export interface SuiviCommandeLigne {
  contratId: string;
  //   Logiciel effectif : celui du contrat, sinon déduit du planning
  applicationId?: string | null;
  applicationNom?: string | null;
  applicationSource: SourceLogiciel;
  logicielsPlanning: { id: string; nom: string }[];
  prestataireImageUrl?: string | null;
  numeroCommande?: string | null;
  tranche: TrancheContrat;
  referenceCtr?: string | null;
  commentaire?: string | null;
  typeContrat?: string | null;
  active: boolean;
  prestataireId: string;
  prestataireNom: string;
  prestataireActif: boolean;
  entrepriseNom?: string | null;
  entrepriseCode?: string | null;
  tjm: number;
  nombreJours: number;
  montantTotal: number;
  dateDebut: string;
  dateFin: string;
  repartitionAuto: boolean;
  annees: SuiviAnnee[];
  joursConsommes: number;
  montantConsomme: number;
  joursRestants: number;
  montantRestant: number;
}

export interface SuiviApplication {
  id: string;
  nom: string;
  couleur: string;
  statut: string;
  ordre: number;
}

export interface SuiviCommandes {
  applications: SuiviApplication[];
  lignes: SuiviCommandeLigne[];
}

@Injectable({
  providedIn: 'root'
})
export class ContratService {
  private apiUrl = `${environment.apiUrl}/contrats`;

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

  getAll(): Observable<ContratDto[]> {
    return this.http.get<ContratDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getById(id: string): Observable<ContratDto> {
    return this.http.get<ContratDto>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(contrat: ContratDto): Observable<any> {
    const payload: any = {
      ...contrat,
      tjm: contrat.tjm ?? 0,
      seuilJoursMois: contrat.seuilJoursMois ?? null,
      active: contrat.active ?? true
    };
    return this.http.post(this.apiUrl, payload, { headers: this.getHeaders() });
  }

  update(id: string, contrat: ContratDto): Observable<any> {
    const payload: any = {
      ...contrat,
      id,
      tjm: contrat.tjm ?? 0,
      seuilJoursMois: contrat.seuilJoursMois ?? null,
      active: contrat.active ?? true
    };
    return this.http.put(`${this.apiUrl}/${id}`, payload, { headers: this.getHeaders() });
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  //   Tableau de suivi des commandes (Admin/Manager)
  getSuiviCommandes(): Observable<SuiviCommandes> {
    return this.http.get<SuiviCommandes>(`${this.apiUrl}/suivi-commandes`, { headers: this.getHeaders() });
  }

  //   Change uniquement le logiciel d'un contrat (null = selon le planning)
  updateApplication(id: string, applicationId: string | null): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/application`, { applicationId }, { headers: this.getHeaders() });
  }

  //   Répartition annuelle proposée (prorata des jours ouvrés de chaque année)
  getRepartitionProposee(dateDebut: string, dateFin: string, nombreJours: number): Observable<RepartitionAnnuelle[]> {
    const params = new HttpParams()
      .set('dateDebut', dateDebut)
      .set('dateFin', dateFin)
      .set('nombreJours', String(nombreJours));
    return this.http.get<RepartitionAnnuelle[]>(`${this.apiUrl}/repartition-proposee`, { headers: this.getHeaders(), params });
  }

  // ✅ NOUVEAU : Activer / Désactiver un contrat
  toggleActive(id: string, active: boolean): Observable<any> {
    return this.http.patch(
      `${this.apiUrl}/${id}/toggle-active`,
      { active },
      { headers: this.getHeaders() }
    );
  }

  // ✅ Calculer les jours restants sur un contrat
  getJoursRestants(contrat: ContratDto): number {
    const utilises = contrat.joursUtilises ?? 0;
    return contrat.nombreJours - utilises;
  }

  // ✅ Statut couleur contrat (pour alerte visuelle)
  // Retourne: 'danger' si dépassé ou date expirée, 'warning' si proche (<= 20j), 'success' sinon
  getContratAlertStatus(contrat: ContratDto): 'danger' | 'warning' | 'success' | 'secondary' {
    if (!(contrat.active ?? true)) return 'secondary'; // Inactif = gris

    // Cas 1 : date de fin expirée
    if (contrat.dateFin) {
      const dateFin = new Date(contrat.dateFin);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (dateFin < today) return 'danger';
    }

    // Cas 2 : jours restants
    const restants = this.getJoursRestants(contrat);
    if (restants < 0) return 'danger';
    if (restants <= 20) return 'warning';
    return 'success';
  }

  verifierSeuilMensuel(contrat: ContratDto, joursExistantsDansMois: number, joursAjouter: number): {
    peutAjouter: boolean;
    joursMax: number;
    message: string;
  } {
    if (!contrat.seuilJoursMois || contrat.seuilJoursMois <= 0) {
      return { peutAjouter: true, joursMax: joursAjouter, message: '' };
    }

    const totalApres = joursExistantsDansMois + joursAjouter;

    if (totalApres <= contrat.seuilJoursMois) {
      return { peutAjouter: true, joursMax: joursAjouter, message: '' };
    } else {
      const joursMax = contrat.seuilJoursMois - joursExistantsDansMois;
      if (joursMax <= 0) {
        return {
          peutAjouter: false,
          joursMax: 0,
          message: `⚠️ Seuil mensuel atteint (${contrat.seuilJoursMois}j max). Aucun jour supplémentaire ne peut être ajouté ce mois-ci.`
        };
      } else {
        return {
          peutAjouter: true,
          joursMax: joursMax,
          message: `ℹ️ Seuil mensuel: ${contrat.seuilJoursMois}j max. Vous aviez déjà ${joursExistantsDansMois}j ce mois. Seuls ${joursMax}j ont été ajoutés au lieu de ${joursAjouter}j demandés.`
        };
      }
    }
  }

  uploadDocument(contratId: string, file: File): Observable<{ documentUrl: string }> {
    const token = this.authService.token;
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ documentUrl: string }>(
      `${this.apiUrl}/${contratId}/upload`,
      formData,
      { headers }
    );
  }
}