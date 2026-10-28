export interface Estimation {
  id?: string;
  annee: number;
  budgetCagnotte: number;
  budgetConsomme: number;
  creePar?: string;
  createurNom?: string;
  dateCreation?: Date;
  dateModification?: Date;
  lignes: LigneEstimation[];
}

export interface LigneEstimation {
  id?: string;
  estimationId?: string;
  prestataireId?: string | null;
  nomFuturPrestataire?: string | null;
  mois: string; // Format: "2025-12"
  nombreJours: number;
  tjm: number;
  revenu: number;
  ordre?: number;
  nomPrestataire?: string; // Pour affichage
}

export interface CreateEstimationCommand {
  annee: number;
  budgetCagnotte: number;
  budgetConsomme: number;
  lignes: CreateLigneEstimationDto[];
}

export interface CreateLigneEstimationDto {
  prestataireId?: string | null;
  nomFuturPrestataire?: string | null;
  mois: string;
  nombreJours: number;
  tjm: number;
  ordre?: number;
}

export interface UpdateEstimationCommand {
  budgetCagnotte: number;
  budgetConsomme: number;
  lignes: UpdateLigneEstimationDto[];
}

export interface UpdateLigneEstimationDto {
  id?: string | null;
  prestataireId?: string | null;
  nomFuturPrestataire?: string | null;
  mois: string;
  nombreJours: number;
  tjm: number;
  ordre?: number;
}