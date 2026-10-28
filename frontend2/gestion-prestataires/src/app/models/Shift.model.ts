export enum ShiftCategory {
  Travail = 0,
  Absence = 1,
  Conge = 2
}

export interface TypeShift {
  id: string;
  label: string;
  shortLabel: string;
  code: string;
  color: string;
  bgColor: string;
  borderColor: string;
  textColor: string;
  isActive: boolean;
  order: number;
  category: ShiftCategory;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface Shift {
  id: string;
  prestataireId: string;
  prestataireNom?: string;
  prestatairePrenom?: string;
  date: Date;
  typeShiftId: string;
  typeShift: TypeShift;
  notes?: string;
  createdAt?: Date;
  updatedAt?: Date;
  nombreJours?: number;
}