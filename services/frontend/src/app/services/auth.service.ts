import { Injectable } from '@angular/core';
import { ACCESS_TOKEN_KEY } from '../constants/auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  isLoggedIn(): boolean {
    return typeof sessionStorage !== 'undefined' && !!sessionStorage.getItem(ACCESS_TOKEN_KEY);
  }

  clearSession(): void {
    if (typeof sessionStorage === 'undefined') {
      return;
    }

    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem('pkce_code_verifier');
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
