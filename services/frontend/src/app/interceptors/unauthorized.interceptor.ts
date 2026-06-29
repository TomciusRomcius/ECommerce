import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ACCESS_TOKEN_KEY } from '../constants/auth';
import { environment } from '../../environments/environment';

export const unauthorizedInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        req.url.startsWith(environment.backendApi) &&
        !req.url.endsWith('/token')
      ) {
        sessionStorage.removeItem(ACCESS_TOKEN_KEY);

        if (!router.url.startsWith('/login')) {
          void router.navigate(['/login']);
        }
      }

      return throwError(() => error);
    }),
  );
};
