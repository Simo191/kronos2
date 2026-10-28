export interface Prestataire {
  id?: string;
  nom: string;
  prenom: string;
  email: string;
  telephone?: string;
  tjm: number;
  userId?: string | null;   
  user?: UserBasicInfo;    
  responsable?: boolean;    
  order?: number;
  imageUrl?: string;        //   NOUVEAU : chemin relatif vers l'image avatar
  //   Activation du compte prestataire.
  //   false  => masqué partout (shifts, contrats, sélecteurs...) sauf liste Prestataires (grisé)
  //          + connexion bloquée si User lié
  isActive?: boolean;
  //   Motif et date de la dernière désactivation (réinitialisés à la réactivation)
  deactivationReason?: string | null;
  deactivatedAt?: Date | string | null;
  //   Entreprise / SSII de rattachement (facultative, gérée dans Paramétrage)
  entrepriseId?: string | null;
  entreprise?: { id: string; nom: string; code?: string | null } | null;
  createdAt?: Date;
  updatedAt?: Date;
}

//  Info basique user pour affichage
export interface UserBasicInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
}