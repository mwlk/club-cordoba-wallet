import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialListItem } from '../../../../core/models/credential.model';

// UC02: listado de credenciales emitidas. Si no hay ninguna, se muestra
// el estado vacío (extensión 2a del enunciado). Si el backend falla, se
// muestra un error con reintentar en vez de un falso "sin datos".
@Component({
  selector: 'app-credential-list',
  standalone: false,
  templateUrl: './credential-list.component.html',
  styleUrl: './credential-list.component.scss'
})
export class CredentialListComponent implements OnInit {
  credentials: CredentialListItem[] = [];
  loaded = false;
  errored = false;
  readonly skeletonItems = [0, 1, 2, 3, 4, 5];

  constructor(
    private credentialsService: CredentialsService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loaded = false;
    this.errored = false;
    this.credentialsService.list().subscribe({
      next: list => {
        this.credentials = list;
        this.loaded = true;
        // En este entorno zone.js no parchea XHR/fetch en runtime (verificado),
        // así que NgZone no dispara un tick solo tras un HTTP response. Se
        // fuerza acá (ver docs/decisiones.md, sección Frontend).
        this.cdr.detectChanges();
      },
      error: () => {
        this.errored = true;
        this.loaded = true;
        this.cdr.detectChanges();
      }
    });
  }

  goToCreate(): void {
    this.router.navigate(['/credentials/new']);
  }

  goToDetail(id: string): void {
    this.router.navigate(['/credentials', id]);
  }

  // renovacion-credencial-activa: igual que en credential-card, "vencida"
  // es un estado visual derivado de validUntil, no de credentialStatus
  // (que no se toca al renovar — ver openspec design.md).
  isExpired(item: CredentialListItem): boolean {
    return new Date(item.validUntil) < new Date();
  }
}
