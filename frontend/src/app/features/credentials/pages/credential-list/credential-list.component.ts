import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialListItem } from '../../../../core/models/credential.model';

// UC02: listado de credenciales emitidas. Si no hay ninguna, se muestra
// el estado vacío (extensión 2a del enunciado).
@Component({
  selector: 'app-credential-list',
  standalone: false,
  templateUrl: './credential-list.component.html',
  styleUrl: './credential-list.component.scss'
})
export class CredentialListComponent implements OnInit {
  credentials: CredentialListItem[] = [];
  loaded = false;
  readonly skeletonItems = [0, 1, 2, 3, 4, 5];

  constructor(
    private credentialsService: CredentialsService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.credentialsService.list().pipe(
      catchError(() => of([] as CredentialListItem[]))
    ).subscribe(list => {
      this.credentials = list;
      this.loaded = true;
      // En este entorno zone.js no parchea XHR/fetch en runtime (verificado),
      // así que NgZone no dispara un tick solo tras un HTTP response. Se
      // fuerza acá (ver docs/decisiones.md, sección Frontend).
      this.cdr.detectChanges();
    });
  }

  goToCreate(): void {
    this.router.navigate(['/credentials/new']);
  }

  goToDetail(id: string): void {
    this.router.navigate(['/credentials', id]);
  }
}
