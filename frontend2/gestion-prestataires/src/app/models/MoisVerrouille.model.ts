export interface MoisVerrouille {
    id?: string;
    mois: string; // Format: "2025-12"
    verrouilleParUserId: string;
    verrouilleParUserName?: string;
    dateVerrouillage?: Date;
    commentaire?: string;
  }