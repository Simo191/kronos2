import { Prestataire } from './prestataire.model';
import { TypeContrat } from './type-contrat.model';

export interface Contrat {
  id?: string;
  prestataireId: string;
  typeContratId: string;
  dateDebut: Date;
  dateFin: Date;
  nombreJours: number;
  tjm: number;
  montantTotal: number;
  createdAt?: Date;
  updatedAt?: Date;
  prestataire?: Prestataire;
  typeContrat?: TypeContrat;
  seuilJoursMois?: number | null;
  active: boolean
}
