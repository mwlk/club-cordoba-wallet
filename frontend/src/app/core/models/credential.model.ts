import { MemberCategory, CredentialStatus } from './enums';

export interface CredentialListItem {
  id: string;
  photo: string;
  firstName: string;
  lastName: string;
  category: MemberCategory;
  memberNumber: string;
  validFrom: string;
  validUntil: string;
  status: CredentialStatus;
}

export interface CredentialDetail extends CredentialListItem {
  issuer: string;
  proofType: string;
}
