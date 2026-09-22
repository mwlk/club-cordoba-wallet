import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CredentialsService } from '../../../../core/services/credentials.service';
import { CredentialDetail } from '../../../../core/models/credential.model';

@Component({
  selector: 'app-credential-detail',
  templateUrl: './credential-detail.component.html'
})
export class CredentialDetailComponent implements OnInit {
  credential: CredentialDetail | null = null;
  notFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private credentialsService: CredentialsService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.credentialsService.getById(id).subscribe(response => {
      if (response.success && response.data) {
        this.credential = response.data;
      } else {
        this.notFound = true;
      }
    });
  }

  backToList(): void {
    this.router.navigate(['/credentials']);
  }
}
