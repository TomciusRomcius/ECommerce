import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, of, tap } from 'rxjs';
import { ACCESS_TOKEN_KEY, ADMIN_ROLE } from '../constants/auth';
import ApiResponse from '../models/api-response';
import UserInfo from '../models/user-info';
import { unwrapApiResponse } from '../utils/unwrap-api-response';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly roles = signal<string[]>([]);
  private userLoadStarted = false;

  readonly isAdmin = computed(() => this.roles().includes(ADMIN_ROLE));

  isLoggedIn(): boolean {
    return typeof sessionStorage !== 'undefined' && !!sessionStorage.getItem(ACCESS_TOKEN_KEY);
  }

  ensureUserLoaded(): void {
    if (!this.isLoggedIn() || this.userLoadStarted) {
      return;
    }

    this.userLoadStarted = true;
    this.getUser().subscribe({
      error: () => {
        this.roles.set([]);
        this.userLoadStarted = false;
      },
    });
  }

  getUser(): Observable<UserInfo> {
    if (!this.isLoggedIn()) {
      this.roles.set([]);
      return of({ roles: [] });
    }

    return unwrapApiResponse(
      this.http.get<ApiResponse<UserInfo>>(`${environment.backendApi}/user`),
    ).pipe(
      tap((user) => {
        this.roles.set(user.roles);
        this.userLoadStarted = true;
      }),
    );
  }

  clearSession(): void {
    if (typeof sessionStorage === 'undefined') {
      return;
    }

    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem('pkce_code_verifier');
    this.roles.set([]);
    this.userLoadStarted = false;
  }

  logout(): void {
    this.clearSession();

    const url = new URL(
      'http://localhost:8080/auth/realms/ecommerce-api/protocol/openid-connect/logout',
    );
    url.searchParams.append('client_id', 'frontend');
    url.searchParams.append('post_logout_redirect_uri', 'http://localhost:4200/home');

    globalThis.location.assign(url.toString());
  }
}
