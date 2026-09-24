import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponseDto, CurrentUser, LoginDto, RegisterDto } from './auth.model';

const TOKEN_KEY = 'teamcraft_token';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/auth`;

    readonly currentUser = signal<CurrentUser | null>(this.decodeToken(this.getToken()));

    login(dto: LoginDto): Observable<AuthResponseDto> {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/login`, dto)
            .pipe(tap(response => this.setSession(response)));
    }

    register(dto: RegisterDto): Observable<AuthResponseDto> {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/register`, dto)
            .pipe(tap(response => this.setSession(response)));
    }

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);
        this.currentUser.set(null);
    }

    getToken(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }

    isLoggedIn(): boolean {
        return this.currentUser() !== null;
    }

    private setSession(response: AuthResponseDto): void {
        localStorage.setItem(TOKEN_KEY, response.token);
        this.currentUser.set({ email: response.email, role: response.role });
    }

    private decodeToken(token: string | null): CurrentUser | null {
        if (!token) return null;

        try {
            const payload = JSON.parse(atob(token.split('.')[1]));

            const email = payload['email'] ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
            const role = payload['role'] ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

            if (!email || !role) return null;

            return { email, role };
        } catch {
            return null;
        }
    }
}
