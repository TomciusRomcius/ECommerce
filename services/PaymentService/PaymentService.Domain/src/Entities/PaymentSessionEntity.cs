using PaymentService.Domain.src.Enums;

namespace PaymentService.Domain.src.Entities
{
    public class PaymentSessionEntity
    {
        public required string PaymentSessionId { get; set; }
        public required string CheckoutUrl { get; set; }

        public required Guid UserId { get; set; }
        public required PaymentProvider PaymentSessionProvider { get; set; }
    }
}
