import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { MemberCategory } from '../../../../core/models/enums';
import { MemberSearchResult } from '../../../../core/models/dtos/member-search.response';
import { CanComponentDeactivate } from '../../../../core/guards/unsaved-changes.guard';
import { CreateCredentialResult } from '../../../../core/models/dtos/create-credential.response';

// UC01: formulario de alta, campos tal cual la sección 4.1.3 del enunciado
// (nombre, apellido, DNI, categoría, foto). El buscador de socio es UX
// sobre el mismo POST -> no hay endpoint de alta de socio separado.
@Component({
  selector: 'app-credential-create',
  standalone: false,
  templateUrl: './credential-create.component.html',
  styleUrl: './credential-create.component.scss'
})
export class CredentialCreateComponent implements CanComponentDeactivate {
  categories = Object.values(MemberCategory);
  submitting = false;
  result: CreateCredentialResult | null = null;
  memberFound = false;

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private credentialsService: CredentialsService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      dni: ['', [Validators.required, Validators.pattern(/^\d{7,8}$/)]],
      nombre: ['', Validators.required],
      apellido: ['', Validators.required],
      categoria: [MemberCategory.Adulto, Validators.required],
      foto: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]]
    });
  }

  onDniChange(dni: string): void {
    this.form.patchValue({ dni });
  }

  onMemberFound(member: MemberSearchResult): void {
    this.memberFound = true;
    this.form.patchValue({ dni: member.dni, nombre: member.firstName, apellido: member.lastName });
    this.form.get('nombre')?.disable();
    this.form.get('apellido')?.disable();
  }

  onMemberNotFound(): void {
    this.memberFound = false;
    this.form.get('nombre')?.enable();
    this.form.get('apellido')?.enable();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    const raw = this.form.getRawValue();

    this.credentialsService.create({
      nombre: raw.nombre!,
      apellido: raw.apellido!,
      dni: raw.dni!,
      categoria: raw.categoria!,
      foto: raw.foto!
    }).subscribe({
      next: response => {
        this.submitting = false;
        if (response.success && response.data) {
          this.result = response.data;
        }
        // En este entorno zone.js no parchea XHR/fetch en runtime (verificado),
        // así que NgZone no dispara un tick solo tras un HTTP response. Se
        // fuerza acá (ver docs/decisiones.md, sección Frontend).
        this.cdr.detectChanges();
      },
      error: () => {
        // El snackbar global (error.interceptor.ts) ya muestra el mensaje;
        // acá solo liberamos el formulario para reintentar.
        this.submitting = false;
        this.cdr.detectChanges();
      }
    });
  }

  backToList(): void {
    // El toast de éxito lo muestra success.interceptor.ts (mensaje real del
    // backend), ya no hace falta armarlo acá.
    this.router.navigate(['/credentials']);
  }

  hasUnsavedChanges(): boolean {
    return this.form.dirty && !this.result;
  }
}
