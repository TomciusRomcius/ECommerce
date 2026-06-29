using System.ComponentModel.DataAnnotations;

namespace BFF.Auth;

public class KeycloakAuthOptions
{
    public const string SectionName = "KeycloakAuth";
    [Required]
    public required string TokenEndpoint { get; set; }
    [Required]
    public required string ClientId { get; set; }
    [Required]
    public required string RedirectUri { get; set; }
}
