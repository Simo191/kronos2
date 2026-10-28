export interface ApplicationPlanning {
    id?: string;
    nom: string;
    description?: string;
    dateDebut: string; // ISO date string
    dateFin: string;
    statut: 'Planifié' | 'En cours' | 'Terminé' | 'En pause';
    priorite: 'Faible' | 'Moyenne' | 'Haute' | 'Critique';
    couleur: string;
    avancement: number;
    dateCreation?: string;
    dateModification?: string;
    prestataires: AssignmentDto[];
    servers?: ApplicationServerDto[];
  }

  export interface AssignmentDto {
    prestataireId: string;
    nomComplet: string;
    role?: string;
  }

  export interface AssignmentInputDto {
    prestataireId: string;
    role?: string;
  }

  //   Type d'environnement supporté côté backend (normalisé en MAJUSCULES)
  export type ServerType = 'DEV' | 'RECETTE' | 'PROD' | 'INT' | 'AUTRE';

  export interface ApplicationServerDto {
    id?: string;
    type: ServerType;
    identifiant: string;
    libelle?: string | null;
    note?: string | null;
    ordre?: number;
  }

  export interface ApplicationServerInputDto {
    type: ServerType;
    identifiant: string;
    libelle?: string | null;
    note?: string | null;
    ordre?: number;
  }

  export interface CreateApplicationPlanningDto {
    nom: string;
    description?: string;
    dateDebut: string;
    dateFin: string;
    statut: string;
    priorite: string;
    couleur: string;
    avancement: number;
    prestataires: AssignmentInputDto[];
    servers: ApplicationServerInputDto[];
  }

  export type UpdateApplicationPlanningDto = CreateApplicationPlanningDto;