using PaymentService.Application.src.Interfaces;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Infrastructure.src.Services
{
    public class WebhookEventStrategyMap<TStrategyType> : IWebhookEventStrategyMap
    {
        // Stripe EventTypes static class should be used by the client to set the key
        // TODO: use safer strategy types
        private Dictionary<string, TStrategyType> _strategyMap;

        public WebhookEventStrategyMap()
        {
            _strategyMap = new Dictionary<string, TStrategyType>();
        }

        public void AddStrategy<T>(string eventType, T strategy)
        {
            if (typeof(T) != typeof(TStrategyType))
            {
                throw new InvalidOperationException($"T type should be {typeof(TStrategyType)}");
            }

            _strategyMap.Add(eventType, (TStrategyType)(object)strategy!);
        }

        public Result<T> TryGetStrategy<T>(string strategyName)
        {
            if (typeof(T) != typeof(TStrategyType))
            {
                return new Result<T>([
                    new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, $"T type should be {typeof(TStrategyType)}")
                ]);
            }

            _strategyMap.TryGetValue(strategyName, out TStrategyType? strategy);
            if (strategy == null)
            {
                var error = new ResultError(
                    ResultErrorType.UNSUPPORTED,
                    "Webhook event type does not have an associated handler"
                );

                return new Result<T>([error]);
            }

            return new Result<T>((T)(object)strategy);
        }
    }
}
