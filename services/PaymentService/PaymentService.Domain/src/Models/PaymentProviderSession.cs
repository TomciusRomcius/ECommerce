using PaymentService.Domain.src.Enums;

namespace PaymentService.Domain.src.Models;

public class PaymentProviderSession
{
    public required string UserId { get; set; } // Retrieved from metadata
    public required string SessionId { get; set; }
    public required string CheckoutUrl { get; set; }
    public required string Currency { get; set; }
    public required PaymentProvider Provider { get; set; }
}