using Microsoft.AspNetCore.Mvc;

namespace OrderService.Presentation.Controllers;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult ApiStatus()
    {
        return Ok("API working!");
    }
}