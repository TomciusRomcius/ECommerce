using PaymentService.Domain.src.Enums;

namespace PaymentService.Domain.src.Models;

public class PaymentProviderEvent
{
    public required PaymentProvider Provider { get; set; }
    public required string EventType { get; set; }
    public required Dictionary<string, object> Data { get; set; }
}