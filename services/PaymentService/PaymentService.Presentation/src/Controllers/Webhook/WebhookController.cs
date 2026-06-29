using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.src.Interfaces;

namespace PaymentService.Presentation.src.Controllers.Webhook
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IWebhookCoordinatorService _webhookCoordinator;

        public WebhookController(ILogger<WebhookController> logger, IWebhookCoordinatorService webhookCoordinator)
        {
            _logger = logger;
            _webhookCoordinator = webhookCoordinator;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhookAsync()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string? signature = HttpContext.Request.Headers["Stripe-Signature"];
            if (signature == null)
            {
                return Unauthorized("Signature not provided");
            }
            await _webhookCoordinator.HandlePaymentWebhook(json, signature);
            return Ok();
        }
    }
}
