using PaymentService.Application.src.Models;
using PaymentService.Domain.src.Models;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Application.src.Interfaces;

public class GeneratePaymentSessionOptions
{
    public required Guid UserId { get; init; }
    public required Guid OrderId { get; init; }
    /// <summary>
    ///     Price in cents
    /// </summary>
    public required int Price { get; init; }
}

public interface IProviderPaymentSessionService
{
    public Task<PaymentProviderSession> GeneratePaymentSession(GeneratePaymentSessionOptions sessionOptions);
    public Task<Result<T>> ParseWebhookEvent<T>(string json, string signature);
    public Task<PaymentSessionDetails?> GetPaymentSessionDetails(string sessionId);
}