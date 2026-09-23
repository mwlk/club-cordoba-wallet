import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialDetail } from '../../../../core/models/credential.model';
import { ApiResponse } from '../../../../core/models/api-response.model';

@Component({
  selector: 'app-credential-detail',
  standalone: false,
  templateUrl: './credential-detail.component.html',
  styleUrl: './credential-detail.component.scss'
})
export class CredentialDetailComponent implements OnInit {
  credential: CredentialDetail | null = null;
  notFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private credentialsService: CredentialsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.credentialsService.getById(id).pipe(
      catchError(() => of({ success: false, message: '', data: null } as ApiResponse<CredentialDetail>))
    ).subscribe(response => {
      if (response.success && response.data) {
        this.credential = response.data;
      } else {
        this.notFound = true;
      }
      // En este entorno zone.js no parchea XHR/fetch en runtime (verificado),
      // así que NgZone no dispara un tick solo tras un HTTP response. Se
      // fuerza acá (ver docs/decisiones.md, sección Frontend).
      this.cdr.detectChanges();
    });
  }

  backToList(): void {
    this.router.navigate(['/credentials']);
  }
}
