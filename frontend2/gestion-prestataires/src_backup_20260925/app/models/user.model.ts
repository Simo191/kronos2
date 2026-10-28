export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roleId?: string;
  role: Role;
  isActive: boolean;         // ✅ ACTIVATION/DÉSACTIVATION
  prestataireId?: string | null; // ✅ Prestataire lié (calculé côté frontend)
  createdAt?: Date;
  lastLoginAt?: Date;
}

export interface Role {
  id: string;
  name: string;
  displayName?: string;
  description?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  prestataireId?: string | null; // ✅ Prestataire lié (calculé côté frontend)
  id: string; 
  roleId: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

// ✅ DTO pour création/modification user
export interface UserDto {
  id?: string;
  email: string;
  password?: string;  // Seulement pour création
  firstName: string;
  lastName: string;
  roleId: string;
  isActive: boolean;
  prestataireId?: string | null; // ✅ Pour lier/délier prestataire
}