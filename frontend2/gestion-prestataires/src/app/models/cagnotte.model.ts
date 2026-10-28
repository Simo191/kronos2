export interface SourceBudget {
  id?: string;
  cagnotteId: string;
  nom: string;
  code: string;
  montant: number;
  description?: string;
  dateCreation?: Date;
  dateModification?: Date;
}

export interface Cagnotte {
  id?: string;
  annee: number;
  budgetAnnuel?: number; // Calculé côté serveur
  budgetConsomme: number;
  budgetRestant?: number; // Calculé côté serveur
  sourcesBudget?: SourceBudget[];
  dateCreation?: Date;
  dateModification?: Date;
}