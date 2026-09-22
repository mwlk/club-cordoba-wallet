import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'credentials', pathMatch: 'full' },
  {
    path: 'credentials',
    loadChildren: () => import('./features/credentials/credentials.module').then(m => m.CredentialsModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
