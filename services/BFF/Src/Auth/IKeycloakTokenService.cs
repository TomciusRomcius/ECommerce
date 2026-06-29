namespace BFF.Auth;

public interface IKeycloakTokenService
{
    Task<string> ExchangeAuthorizationCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken cancellationToken = default);
}
