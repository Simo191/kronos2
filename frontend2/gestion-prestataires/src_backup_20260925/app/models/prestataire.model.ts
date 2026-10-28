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