using ECommerce.Presentation.src.Utils;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.src.Interfaces;
using PaymentService.Domain.src.Entities;
using PaymentService.Domain.src.Enums;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Presentation.src.Controllers.PaymentSession
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentSessionController : ControllerBase
    {
        private readonly IPaymentSessionCoordinator _paymentCoordinator;

        public PaymentSessionController(IPaymentSessionCoordinator paymentCoordinator)
        {
            _paymentCoordinator = paymentCoordinator;
        }

        //TODO: JWT auth
        [HttpGet]
        public async Task<IActionResult> GetPaymentSessionAsync([FromQuery] Guid userId)
        {
            PaymentSessionEntity? result = await _paymentCoordinator.GetUserSessionAsync(userId);
            return Ok(result);
        }

        // TODO: JWT auth
        [HttpPost]
        public async Task<IActionResult> CreatePaymentSession([FromBody] CreatePaymentSessionDto dto)
        {
            // TODO: error handling

            var options = new GeneratePaymentSessionOptions
            {
                UserId = dto.UserId,
                OrderId = dto.OrderId,
                Price = dto.PriceCents,
            };
            Result<PaymentSessionEntity?> result = await _paymentCoordinator.CreatePaymentSessionAsync(dto.PaymentProvider, options);

            if (result.Errors.Any())
            {
                return ControllerUtils.ResultErrorToResponse(result.Errors.First());
            }

            return Created("", result.GetValue());
        }

        [HttpPost("verify/stripe")]
        public async Task<IActionResult> VerifyStripePaymentAsync([FromBody] VerifyStripePaymentDto dto)
        {
            bool hasPaid = await _paymentCoordinator.VerifyPaymentAsync(PaymentProvider.STRIPE, dto.SessionId);
            return Ok(new { hasPaid });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid userId)
        {
            ResultError? error = await _paymentCoordinator.DeleteUserSessionAsync(userId);
            if (error is not null)
            {
                return ControllerUtils.ResultErrorToResponse(error);
            }

            return NoContent();
        }
    }
}
