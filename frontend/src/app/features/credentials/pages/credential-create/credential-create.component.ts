import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { MemberCategory } from '../../../../core/models/enums';
import { MemberSearchResult } from '../../../../core/models/dtos/member-search.response';
import { CanComponentDeactivate } from '../../../../core/guards/unsaved-changes.guard';
import { CreateCredentialResult } from '../../../../core/models/dtos/create-credential.response';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';

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
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
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

  async submit(): Promise<void> {
    // sdd-review renovacion-credencial-activa (WARNING 3): `submitting` se
    // marca ACÁ, antes del chequeo async de credencial activa y del popup
    // -> el botón queda deshabilitado durante toda esa ventana, no solo
    // durante el POST. Sin esto, un doble click disparaba dos chequeos/dos
    // popups en paralelo.
    if (this.form.invalid || this.submitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.cdr.detectChanges();

    const raw = this.form.getRawValue();
    const dni = raw.dni!;

    // renovacion-credencial-activa: el chequeo va por el valor del campo
    // DNI del form, no por `memberFound` — un DNI existente tipeado a mano
    // (sin clickear ningún candidato de la búsqueda) también debe disparar
    // el popup si ese socio ya tiene una credencial vigente.
    let confirmarRenovacion = false;
    try {
      const activeCheck = await firstValueFrom(this.credentialsService.getActiveCredential(dni));
      if (activeCheck.success && activeCheck.data) {
        const validUntil = new Date(activeCheck.data.validUntil);
        const validUntilLabel = validUntil.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });

        const confirmed = await firstValueFrom(this.dialog.open(ConfirmationDialogComponent, {
          data: {
            title: 'Renovar credencial',
            message: `Este socio ya tiene una credencial vigente hasta el ${validUntilLabel}. ¿Generar una nueva? La credencial actual quedará vencida desde hoy.`
          }
        }).afterClosed());

        if (!confirmed) {
          this.submitting = false;
          this.cdr.detectChanges();
          return;
        }
        confirmarRenovacion = true;
      }
    } catch {
      // Chequeo previo falló (red/500): el snackbar global ya informa el
      // error (error.interceptor.ts) -> se libera el formulario para
      // reintentar en vez de dejarlo colgado en "Emitiendo…".
      this.submitting = false;
      this.cdr.detectChanges();
      return;
    }

    this.credentialsService.create({
      nombre: raw.nombre!,
      apellido: raw.apellido!,
      dni,
      categoria: raw.categoria!,
      foto: raw.foto!,
      confirmarRenovacion
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
