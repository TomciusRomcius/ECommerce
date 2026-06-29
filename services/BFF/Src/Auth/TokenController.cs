using Microsoft.AspNetCore.Mvc;

namespace BFF.Auth;

[ApiController]
[Route("[controller]")]
public class TokenController(IKeycloakTokenService keycloakTokenService, ILogger<TokenController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Exchange(
        [FromBody] TokenExchangeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.CodeVerifier))
        {
            return BadRequest(new { error = "Code and codeVerifier are required." });
        }

        try
        {
            string accessToken = await keycloakTokenService.ExchangeAuthorizationCodeAsync(
                request.Code,
                request.CodeVerifier,
                cancellationToken);
            
            return Ok(new { data = accessToken });
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Token exchange failed.");
            return BadRequest(new { error = "Token exchange failed." });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Token exchange returned an invalid response.");
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Invalid token response from identity provider." });
        }
    }
}
