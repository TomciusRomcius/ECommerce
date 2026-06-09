using Microsoft.AspNetCore.Mvc;

namespace OrderService.Presentation;

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