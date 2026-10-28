import { Prestataire } from './prestataire.model';

export interface JourTravail {
  id?: string;
  prestataireId: string;
  mois: string; // Format: "2025-12"
  nombreJours: number;
  description?: string;
  createdAt?: Date;
  tjm: number;
  prestataire?: Prestataire;
}
