import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const token = authService.getToken();

    if (token) {
        req = req.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
        });
    }

    // Un 401 sul login/registrazione sono credenziali errate, non una sessione scaduta:
    // lo gestisce già il componente stesso, qui non deve scatenare un redirect.
    const isAuthRequest = req.url.includes('/auth/login') || req.url.includes('/auth/register');

    return next(req).pipe(
        catchError((error: unknown) => {
            if (error instanceof HttpErrorResponse && error.status === 401 && !isAuthRequest) {
                authService.logout();
                router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
            }

            return throwError(() => error);
        })
    );
};
