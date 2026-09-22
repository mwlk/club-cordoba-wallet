import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CredentialListComponent } from './pages/credential-list/credential-list.component';
import { CredentialCreateComponent } from './pages/credential-create/credential-create.component';
import { CredentialDetailComponent } from './pages/credential-detail/credential-detail.component';
import { unsavedChangesGuard } from '../../core/guards/unsaved-changes.guard';

const routes: Routes = [
  { path: '', component: CredentialListComponent },
  { path: 'new', component: CredentialCreateComponent, canDeactivate: [unsavedChangesGuard] },
  { path: ':id', component: CredentialDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CredentialsRoutingModule {}
