using System.ComponentModel.DataAnnotations;

namespace PaymentService.Infrastructure.src.Utils;

public class StripeSettings
{
    public required string ApiKey { get; init; }
    public required string WebhookSecret { get; init; }
    [Required]
    public required string CheckoutSuccessUrl { get; init; } = "http://localhost:4200/checkout?payment-success";
    [Required]
    public required string CheckoutCancelUrl { get; init; } = "http://localhost:4200/checkout?payment-cancelled";
}