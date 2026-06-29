namespace BFF.Auth;

public class TokenExchangeRequest
{
    public required string Code { get; set; }
    public required string CodeVerifier { get; set; }
}
