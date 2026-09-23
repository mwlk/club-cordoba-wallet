import { MemberCategory } from '../enums';

export interface CreateCredentialRequest {
  nombre: string;
  apellido: string;
  dni: string;
  categoria: MemberCategory;
  foto: string;
  confirmarRenovacion?: boolean;
}
