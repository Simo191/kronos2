/**
 *   Compte technique d'un prestataire (ADM, ADML, VPN, Bastion…).
 *   Le mot de passe N'est JAMAIS envoyé par le backend dans cette structure :
 *   il faut passer par le service `reveal()` qui exige le mot de passe utilisateur.
 */
export interface PrestataireCredential {
  id: string;
  prestataireId: string;
  label: string;
  accountName: string;
  expiresAt?: string | null;
  note?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreatePrestataireCredentialDto {
  label: string;
  accountName: string;
  password: string;
  expiresAt?: string | null;
  note?: string | null;
}

export interface UpdatePrestataireCredentialDto {
  label: string;
  accountName: string;
  /** Vide => on garde l'ancien mdp chiffré côté serveur. */
  password?: string | null;
  expiresAt?: string | null;
  note?: string | null;
}

export interface RevealCredentialResponse {
  id: string;
  password: string;
}
