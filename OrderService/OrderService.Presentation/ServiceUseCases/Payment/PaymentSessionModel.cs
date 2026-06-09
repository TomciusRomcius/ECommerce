namespace OrderService.Presentation.ServiceUseCases.Payment
{
    public class PaymentSessionModel
    {
        public required string PaymentSessionId { get; set; }
        public required string CheckoutUrl { get; set; }
        public required Guid UserId { get; set; }
        public required PaymentProvider PaymentSessionProvider { get; set; }
    }
}