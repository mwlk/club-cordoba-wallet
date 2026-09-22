import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CredentialsRoutingModule } from './credentials-routing.module';

import { CredentialListComponent } from './pages/credential-list/credential-list.component';
import { CredentialCreateComponent } from './pages/credential-create/credential-create.component';
import { CredentialDetailComponent } from './pages/credential-detail/credential-detail.component';
import { MemberSearchComponent } from './components/member-search/member-search.component';

@NgModule({
  declarations: [
    CredentialListComponent,
    CredentialCreateComponent,
    CredentialDetailComponent,
    MemberSearchComponent
  ],
  imports: [SharedModule, CredentialsRoutingModule]
})
export class CredentialsModule {}
