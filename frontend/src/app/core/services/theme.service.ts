import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';

export type Theme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly STORAGE_KEY = 'cc-theme';

  private readonly theme = new BehaviorSubject<Theme>(this.initial());

  readonly isDark$: Observable<boolean> = this.theme.pipe(map((t) => t === 'dark'));

  constructor() {
    this.apply(this.theme.value);
  }

  toggle(): void {
    const next: Theme = this.theme.value === 'dark' ? 'light' : 'dark';
    this.theme.next(next);
    try {
      localStorage.setItem(this.STORAGE_KEY, next);
    } catch {
      /* almacenamiento restringido: el tema sigue activo en esta sesión */
    }
    this.apply(next);
  }

  private apply(theme: Theme): void {
    document.documentElement.setAttribute('data-theme', theme);
  }

  private initial(): Theme {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (stored === 'light' || stored === 'dark') {
        return stored;
      }
    } catch {
      /* sin acceso al almacenamiento: se usa la preferencia del sistema */
    }
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
}