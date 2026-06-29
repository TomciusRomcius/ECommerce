using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace BFF.Auth;

public class KeycloakTokenService(
    HttpClient httpClient,
    IOptions<KeycloakAuthOptions> options,
    ILogger<KeycloakTokenService> logger) : IKeycloakTokenService
{
    public async Task<string> ExchangeAuthorizationCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken cancellationToken = default)
    {
        KeycloakAuthOptions authOptions = options.Value;

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", authOptions.ClientId },
            { "code", code },
            { "redirect_uri", authOptions.RedirectUri },
            { "code_verifier", codeVerifier },
        });

        using HttpResponseMessage response = await httpClient.PostAsync(
            authOptions.TokenEndpoint,
            content,
            cancellationToken);
        logger.LogInformation("AA {@a}", await response.Content.ReadAsStringAsync(cancellationToken));
        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Keycloak token exchange failed with status {StatusCode}: {ErrorBody}",
                response.StatusCode,
                errorBody);
            throw new HttpRequestException(
                $"Keycloak token exchange failed with status {(int)response.StatusCode}.");
        }

        KeycloakTokenResponse? tokenResponse =
            await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);

        if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
        {
            throw new InvalidOperationException("Keycloak token response did not include an access token.");
        }

        return tokenResponse.AccessToken;
    }
}
