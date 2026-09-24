import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';

import { CredentialCardComponent } from './components/credential-card/credential-card.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { MemberAvatarComponent } from './components/member-avatar/member-avatar.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { CredentialStatusPipe } from './pipes/credential-status.pipe';
import { MemberCategoryPipe } from './pipes/member-category.pipe';

const MATERIAL_MODULES = [
  MatButtonModule, MatCardModule, MatDialogModule, MatFormFieldModule,
  MatIconModule, MatInputModule, MatProgressSpinnerModule, MatProgressBarModule,
  MatSelectModule, MatSnackBarModule, MatToolbarModule
];

@NgModule({
  declarations: [
    CredentialCardComponent,
    EmptyStateComponent,
    ConfirmationDialogComponent,
    LoadingSpinnerComponent,
    MemberAvatarComponent,
    ToolbarComponent,
    CredentialStatusPipe,
    MemberCategoryPipe
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ...MATERIAL_MODULES],
  exports: [
    CommonModule, FormsModule, ReactiveFormsModule, ...MATERIAL_MODULES,
    CredentialCardComponent, EmptyStateComponent, ConfirmationDialogComponent,
    LoadingSpinnerComponent, MemberAvatarComponent, ToolbarComponent, CredentialStatusPipe, MemberCategoryPipe
  ]
})
export class SharedModule {}
