import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialListItem } from '../../../../core/models/credential.model';

// UC02: listado de credenciales emitidas. Si no hay ninguna, se muestra
// el estado vacío (extensión 2a del enunciado).
@Component({
  selector: 'app-credential-list',
  templateUrl: './credential-list.component.html'
})
export class CredentialListComponent implements OnInit {
  credentials: CredentialListItem[] = [];
  loaded = false;

  constructor(private credentialsService: CredentialsService, private router: Router) {}

  ngOnInit(): void {
    this.credentialsService.list().subscribe(list => {
      this.credentials = list;
      this.loaded = true;
    });
  }

  goToCreate(): void {
    this.router.navigate(['/credentials/new']);
  }

  goToDetail(id: string): void {
    this.router.navigate(['/credentials', id]);
  }
}
