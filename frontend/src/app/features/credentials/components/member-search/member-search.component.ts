import { Component, EventEmitter, Output } from '@angular/core';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { MemberSearchResult } from '../../../../core/models/dtos/member-search.response';

// UX de autocompletado sobre GET /credentials/members/search?dni=.
// No es alta de socio: solo informa si existe, para prellenar el form.
// El alta real (si hace falta) ocurre de forma transparente en el POST.
@Component({
  selector: 'app-member-search',
  templateUrl: './member-search.component.html'
})
export class MemberSearchComponent {
  @Output() memberFound = new EventEmitter<MemberSearchResult>();
  @Output() memberNotFound = new EventEmitter<void>();
  @Output() dniChange = new EventEmitter<string>();

  dni = '';
  searching = false;
  resultMessage = '';
  found = false;

  constructor(private credentialsService: CredentialsService) {}

  search(): void {
    if (!this.dni || this.dni.length < 7) return;

    this.dniChange.emit(this.dni);
    this.searching = true;
    this.credentialsService.searchMemberByDni(this.dni).subscribe(response => {
      this.searching = false;
      this.found = response.success;
      this.resultMessage = response.message ?? '';

      if (response.success && response.data) {
        this.memberFound.emit(response.data);
      } else {
        this.memberNotFound.emit();
      }
    });
  }
}
