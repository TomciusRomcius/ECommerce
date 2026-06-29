using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.Payment;

public class GeneratePaymentSessionOptions
{
    public required string UserId { get; set; }
    public required int PriceCents { get; set; }
    public required PaymentProvider PaymentProvider { get; set; }
}

public interface IPaymentSessionService
{
    Task<Result<PaymentSessionModel>> GeneratePaymentSessionAsync(
        Guid orderId,
        GeneratePaymentSessionOptions sessionOptions);

    Task<Result<PaymentSessionModel?>> GetPaymentSessionAsync(Guid userId);
    Task<ResultError?> DeletePaymentSessionAsync(Guid userId);
}
