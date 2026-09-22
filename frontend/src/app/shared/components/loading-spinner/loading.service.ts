import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private requestCount = 0;
  private loading$ = new BehaviorSubject<boolean>(false);
  readonly isLoading$ = this.loading$.asObservable();

  show(): void {
    this.requestCount++;
    this.loading$.next(true);
  }

  hide(): void {
    this.requestCount = Math.max(0, this.requestCount - 1);
    if (this.requestCount === 0) this.loading$.next(false);
  }
}
