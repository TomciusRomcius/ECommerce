using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Auth;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetUser()
    {
        string[] roles = User.FindAll("roles")
            .Select(claim => claim.Value)
            .Concat(User.FindAll(ClaimTypes.Role).Select(claim => claim.Value))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return Ok(new { data = new UserInfoResponse(roles) });
    }
}
