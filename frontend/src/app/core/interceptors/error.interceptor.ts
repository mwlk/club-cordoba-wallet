import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

// Solo maneja errores HTTP reales (5xx, fallos de red, 400 de validación).
// El caso "socio no encontrado" NUNCA pasa por acá: el backend responde
// 200 con { success: false }, por decisión explícita (Result pattern).
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = error.error?.message ?? 'Ocurrió un error de conexión.';
      snackBar.open(message, 'Cerrar', { duration: 4000 });
      return throwError(() => error);
    })
  );
};
