import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialDetail } from '../../../../core/models/credential.model';
import { ApiResponse } from '../../../../core/models/api-response.model';

// UC03: detalle de una credencial. "No encontrada" (Result pattern con
// success:false) y "error de conexión" se tratan distinto: lo segundo
// ofrece reintentar, porque no implica que la credencial no exista.
@Component({
  selector: 'app-credential-detail',
  standalone: false,
  templateUrl: './credential-detail.component.html',
  styleUrl: './credential-detail.component.scss'
})
export class CredentialDetailComponent implements OnInit {
  credential: CredentialDetail | null = null;
  notFound = false;
  errored = false;
  private id = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private credentialsService: CredentialsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.load();
  }

  load(): void {
    this.credential = null;
    this.notFound = false;
    this.errored = false;
    this.credentialsService.getById(this.id).subscribe({
      next: response => {
        if (response.success && response.data) {
          this.credential = response.data;
        } else {
          this.notFound = true;
        }
        // En este entorno zone.js no parchea XHR/fetch en runtime (verificado),
        // así que NgZone no dispara un tick solo tras un HTTP response. Se
        // fuerza acá (ver docs/decisiones.md, sección Frontend).
        this.cdr.detectChanges();
      },
      error: () => {
        this.errored = true;
        this.cdr.detectChanges();
      }
    });
  }

  backToList(): void {
    this.router.navigate(['/credentials']);
  }
}
