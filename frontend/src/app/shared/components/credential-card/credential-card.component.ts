import { Component, Input } from '@angular/core';
import { CredentialDetail } from '../../../core/models/credential.model';

// Se usa tanto en la pantalla de confirmación post-alta (sin sección de
// seguridad) como en el detalle del listado (con ella). Ver docs/decisiones.md
// sobre por qué acá no se muestra JSON crudo ni términos técnicos (DID,
// HMAC) sin traducir: el admin del club no tiene por qué conocerlos.
@Component({
  selector: 'app-credential-card',
  templateUrl: './credential-card.component.html',
  styleUrl: './credential-card.component.scss'
})
export class CredentialCardComponent {
  @Input({ required: true }) credential!: CredentialDetail;
  @Input() showSecurityDetails = true;

  securityExpanded = false;

  toggleSecurity(): void {
    this.securityExpanded = !this.securityExpanded;
  }

  get initials(): string {
    return `${this.credential.firstName[0]}${this.credential.lastName[0]}`.toUpperCase();
  }
}
