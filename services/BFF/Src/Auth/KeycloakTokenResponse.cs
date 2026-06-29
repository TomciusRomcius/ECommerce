using System.Text.Json.Serialization;

namespace BFF.Auth;

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
