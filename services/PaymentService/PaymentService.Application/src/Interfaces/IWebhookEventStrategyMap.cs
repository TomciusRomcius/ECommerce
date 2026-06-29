using PaymentService.Domain.src.Utils;

namespace PaymentService.Application.src.Interfaces
{
    public interface IWebhookEventStrategyMap
    {
        void AddStrategy<T>(string eventType, T strategy);
        Result<T> TryGetStrategy<T>(string strategyName);
    }
}