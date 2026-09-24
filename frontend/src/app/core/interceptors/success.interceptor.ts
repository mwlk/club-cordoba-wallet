import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { tap } from 'rxjs';

const MUTATING_METHODS = ['POST', 'PUT', 'DELETE'];

// Simétrico a error.interceptor.ts: en vez de que cada componente arme su
// propio snackbar de éxito, se centraliza acá usando el `message` real que
// el backend ya devuelve vía el Result pattern (`{success, message, data}`).
// Sólo dispara en escritura (POST/PUT/DELETE) 2xx con success:true — un GET
// no es una "operación" que amerite feedback positivo, y un 2xx con
// success:false (no debería pasar hoy, pero no se asume) tampoco.
export const successInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    tap(event => {
      if (!(event instanceof HttpResponse) || !MUTATING_METHODS.includes(req.method)) return;
      if (event.status < 200 || event.status >= 300) return;

      const body = event.body as { success?: boolean; message?: string } | null;
      if (body?.success === true && body.message) {
        snackBar.open(body.message, 'Cerrar', { duration: 4000, panelClass: 'cc-snack-success' });
      }
    })
  );
};
