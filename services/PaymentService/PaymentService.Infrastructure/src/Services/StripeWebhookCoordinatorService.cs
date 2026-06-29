using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentService.Application.src.Interfaces;
using PaymentService.Application.src.Services;
using PaymentService.Domain.src.Enums;
using PaymentService.Domain.src.Utils;
using PaymentService.Infrastructure.src.Interfaces;
using Stripe;

namespace PaymentService.Infrastructure.src.Services;

public class StripeWebhookCoordinatorService : IWebhookCoordinatorService
{
    private readonly WebhookEventStrategyMapContainer _strategyMaps;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<StripeWebhookCoordinatorService> _logger;

    public StripeWebhookCoordinatorService(WebhookEventStrategyMapContainer strategyMaps,
        IServiceScopeFactory serviceScopeFactory,
        IBackgroundTaskQueue backgroundTaskQueue,
        ILogger<StripeWebhookCoordinatorService> logger)
    {
        _strategyMaps = strategyMaps;
        _serviceScopeFactory = serviceScopeFactory;
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
    }

    public async Task HandlePaymentWebhook(string json, string signature)
    {
        _logger.LogTrace("Entered HandlePaymentWebhook");
        using IServiceScope? scope = _serviceScopeFactory.CreateScope();
        var paymentSessionFactory = scope.ServiceProvider.GetRequiredService<IPaymentSessionFactory>();

        IProviderPaymentSessionService paymentSessionService =
            paymentSessionFactory.CreatePaymentSessionService(PaymentProvider.STRIPE);

        Result<Event> result = await paymentSessionService.ParseWebhookEvent<Event>(json, signature);
        if (result.Errors.Any())
        {
            _logger.LogError(result.ErrorsToString());
            return;
        }

        _logger.LogTrace("Succesfully parsed webhook event");

        Event paymentEvent = result.GetValue();
        IWebhookEventStrategyMap? strategyMap = _strategyMaps.GetStrategyMap(PaymentProvider.STRIPE);
        if (strategyMap == null)
        {
            _logger.LogError("Failed to retrieve Stripe webhook strategy map");
            return;
        }

        _logger.LogTrace("Succesfully retrieved Stripe webhook event strategy map");

        Result<IStripeWebhookStrategy> webhookHandlerStrategyResult = strategyMap.TryGetStrategy<IStripeWebhookStrategy>(paymentEvent.Type);
        if (webhookHandlerStrategyResult.Errors.Any())
        {
            ResultError firstError = webhookHandlerStrategyResult.Errors.First();
            if (firstError.ErrorType == ResultErrorType.UNSUPPORTED
                && webhookHandlerStrategyResult.Errors.Count() == 1)
            {
                _logger.LogWarning(firstError.Message);
            }
            else
            {
                _logger.LogError(webhookHandlerStrategyResult.ErrorsToString());
            }
            return;
        };

        _logger.LogTrace("Succesfully retrieved webhook handler strategy for event type: {EventType}", paymentEvent.Type);

        IStripeWebhookStrategy webhookHandlerStrategy = webhookHandlerStrategyResult.GetValue();
        ResultError? runnerError = await webhookHandlerStrategy.RunAsync(paymentEvent.Data.Object);
        if (runnerError != null)
        {
            _logger.LogError(
                "Encountered an error when running a webhook strategy with event type: {EventType}",
                runnerError.Message
            );
            return;
        }

        _logger.LogDebug(
            "Succesfully handled stripe webhook event with type: {EventType}",
            paymentEvent.Type
        );
    }
}