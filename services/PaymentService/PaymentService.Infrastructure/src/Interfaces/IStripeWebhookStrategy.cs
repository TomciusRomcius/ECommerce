using PaymentService.Domain.src.Utils;
using Stripe;

namespace PaymentService.Infrastructure.src.Interfaces
{
    public interface IStripeWebhookStrategy
    {
        string EventType { get; }
        Task<ResultError?> RunAsync(IHasObject ev);
    }
}