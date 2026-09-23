import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { AbstractControl, FormControl } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, startWith, switchMap, tap } from 'rxjs/operators';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { MemberSearchResult } from '../../../../core/models/dtos/member-search.response';

const MIN_PREFIX_LENGTH = 3;

// UX de autocompletado sobre GET /credentials/members/search?dni= (búsqueda
// por prefijo, hasta 10 candidatos). No es alta de socio: solo ayuda a
// prellenar el form eligiendo de una lista. El alta real (si hace falta)
// ocurre de forma transparente en el POST.
@Component({
  selector: 'app-member-search',
  standalone: false,
  templateUrl: './member-search.component.html',
  styleUrl: './member-search.component.scss'
})
export class MemberSearchComponent {
  @Output() memberFound = new EventEmitter<MemberSearchResult>();
  @Output() memberNotFound = new EventEmitter<void>();
  @Output() dniChange = new EventEmitter<string>();

  // Control del formulario padre que valida el DNI (requerido + 7-8 dígitos).
  // El estado de error se pinta acá para que el campo no quede sin feedback.
  @Input() errorControl: AbstractControl | null = null;

  readonly dniControl = new FormControl('', { nonNullable: true });
  candidates$: Observable<MemberSearchResult[]>;
  justSelected = false;

  constructor(private credentialsService: CredentialsService, private cdr: ChangeDetectorRef) {
    this.candidates$ = this.dniControl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(dni => {
        this.dniChange.emit(dni);
        // La lista sólo se oculta al elegir un candidato; volver a tipear
        // (o pegar otro valor) la vuelve a habilitar.
        this.justSelected = false;
        if (dni.length < MIN_PREFIX_LENGTH) return of<MemberSearchResult[]>([]);
        return this.credentialsService.searchMembers(dni).pipe(
          switchMap(response => of(response.success && response.data ? response.data : [])),
          tap(candidates => {
            if (candidates.length === 0) this.memberNotFound.emit();
            // Igual que en ApiService/otros componentes: zone.js no dispara
            // un tick solo tras este HTTP response en este entorno (ver
            // docs/decisiones.md), así que el `async` pipe necesita ayuda
            // para pintar la lista.
            this.cdr.detectChanges();
          })
        );
      })
    );
  }

  select(member: MemberSearchResult): void {
    this.justSelected = true;
    this.dniControl.setValue(member.dni, { emitEvent: false });
    this.dniChange.emit(member.dni);
    this.memberFound.emit(member);
    // async pipe ya refresca la lista por su cuenta, pero el estado interno
    // del FormControl y `justSelected` cambian sin pasar por HTTP -> no
    // dependen del bug de zone.js documentado en docs/decisiones.md, este
    // detectChanges es sólo para que la UI refleje la selección en el
    // mismo tick del click.
    this.cdr.detectChanges();
  }
}
