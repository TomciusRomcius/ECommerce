import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { PagePadderComponent } from '../../components/page-padder/page-padder.component';
import ApiResponse from '../../models/api-response';
import TokenExchangeRequest from '../../models/token-exchange-request';
import { unwrapApiResponse } from '../../utils/unwrap-api-response';
import { ACCESS_TOKEN_KEY } from '../../constants/auth';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-auth-callback',
  imports: [PagePadderComponent],
  templateUrl: './auth-callback.component.html',
})
export class AuthCallbackComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const code = this.route.snapshot.queryParamMap.get('code');
    const codeVerifier = sessionStorage.getItem('pkce_code_verifier');

    if (!code) {
      this.error.set('Missing authorization code.');
      return;
    }

    if (!codeVerifier) {
      this.error.set('Missing PKCE verifier. Please sign in again.');
      return;
    }

    const body: TokenExchangeRequest = { code, codeVerifier };

    unwrapApiResponse(
      this.http.post<ApiResponse<string>>(`${environment.backendApi}/token`, body),
    ).subscribe({
      next: (token) => {
        sessionStorage.removeItem('pkce_code_verifier');
        sessionStorage.setItem(ACCESS_TOKEN_KEY, token);
        void this.router.navigate(['/home']);
      },
      error: () => {
        this.error.set('Failed to exchange authorization code.');
      },
    });
  }
}
