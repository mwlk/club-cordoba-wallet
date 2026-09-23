import { Pipe, PipeTransform } from '@angular/core';
import { CredentialStatus } from '../../core/models/enums';

@Pipe({ name: 'credentialStatus', standalone: false })
export class CredentialStatusPipe implements PipeTransform {
  private readonly labels: Record<number, string> = {
    [CredentialStatus.Active]: 'Activa',
    [CredentialStatus.Revoked]: 'Revocada',
    [CredentialStatus.Suspended]: 'Suspendida'
  };

  transform(value: CredentialStatus): string {
    return this.labels[value] ?? 'Desconocido';
  }
}
