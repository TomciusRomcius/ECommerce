import { CanActivateFn } from '@angular/router';

function base64UrlEncode(buffer: ArrayBuffer | Uint8Array): string {
  const bytes = buffer instanceof Uint8Array ? buffer : new Uint8Array(buffer);
  const binary = Array.from(bytes, (byte) => String.fromCharCode(byte)).join('');
  return btoa(binary).replaceAll('+', '-').replaceAll('/', '_').replace(/=+$/, '');
}

function generateCodeVerifier(): string {
  const bytes = new Uint8Array(32);
  crypto.getRandomValues(bytes);
  return base64UrlEncode(bytes);
}

async function generateCodeChallenge(verifier: string): Promise<string> {
  const digest = await crypto.subtle.digest('SHA-256', new TextEncoder().encode(verifier));
  return base64UrlEncode(digest);
}

export const loginRedirectGuard: CanActivateFn = async () => {
  const verifier = generateCodeVerifier();
  const challenge = await generateCodeChallenge(verifier);
  sessionStorage.setItem('pkce_code_verifier', verifier);

  const url = new URL(
    'http://localhost:8080/auth/realms/ecommerce-api/protocol/openid-connect/auth',
  );
  url.searchParams.append('client_id', 'frontend');
  url.searchParams.append('response_type', 'code');
  url.searchParams.append('redirect_uri', 'http://localhost:4200/auth/callback');
  url.searchParams.append('scope', 'openid profile email');
  url.searchParams.append('state', crypto.randomUUID());
  url.searchParams.append('code_challenge', challenge);
  url.searchParams.append('code_challenge_method', 'S256');

  globalThis.location.assign(url.toString());
  return false;
};
