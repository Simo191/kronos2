/**
 *   Entreprise / SSII de rattachement d'un prestataire.
 *   Gérée par Admin/Manager via la page Paramétrage.
 */
export interface Entreprise {
  id: string;
  nom: string;
  code?: string | null;
  description?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEntrepriseDto {
  nom: string;
  code?: string | null;
  description?: string | null;
}

export interface UpdateEntrepriseDto extends CreateEntrepriseDto {
  isActive: boolean;
}

//   Vue allégée pour intégration ailleurs (dans Prestataire)
export interface EntrepriseLite {
  id: string;
  nom: string;
  code?: string | null;
}
