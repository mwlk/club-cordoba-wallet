import { Component, Input } from '@angular/core';
import { CredentialDetail } from '../../../core/models/credential.model';

// Se usa tanto en la pantalla de confirmación post-alta (sin sección de
// seguridad) como en el detalle del listado (con ella). Ver docs/decisiones.md
// sobre por qué acá no se muestra JSON crudo ni términos técnicos (DID,
// HMAC) sin traducir: el admin del club no tiene por qué conocerlos.
@Component({
  selector: 'app-credential-card',
  standalone: false,
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

  // renovacion-credencial-activa: al renovar, la credencial vieja queda con
  // validUntil en el pasado pero credentialStatus sigue en "active" (no se
  // toca, ver docs-ia/openspec/.../design.md — ese campo es para revocación
  // real, no para esto). "Vencida" es un estado puramente visual derivado
  // de la fecha, no depende de credentialStatus.
  get isExpired(): boolean {
    return new Date(this.credential.validUntil) < new Date();
  }
}
