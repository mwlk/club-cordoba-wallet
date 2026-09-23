import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-toolbar',
  standalone: false,
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss'
})
export class ToolbarComponent {
  readonly isDark$: Observable<boolean>;

  constructor(private readonly theme: ThemeService) {
    this.isDark$ = this.theme.isDark$;
  }

  toggleTheme(): void {
    this.theme.toggle();
  }
}
