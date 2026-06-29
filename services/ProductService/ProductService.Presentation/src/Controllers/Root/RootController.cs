using Microsoft.AspNetCore.Mvc;

namespace ProductService.Presentation.Controllers.Root;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult ApiStatus()
    {
        return Ok("Api working!");
    }
}