using System.Text.Json;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;
using PaymentService.Domain.src.Utils;
using PaymentService.Infrastructure.src.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentService.Infrastructure.src.Services.WebhookStrategies;

public class CheckoutSessionCompletedStrategy : IStripeWebhookStrategy
{
    private readonly KafkaConfiguration _kafkaConfiguration;

    public CheckoutSessionCompletedStrategy(IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public string EventType => "checkout.session.completed";

    public async Task<ResultError?> RunAsync(IHasObject ev)
    {
        KafkaEventProducer producer = new KafkaEventProducer(_kafkaConfiguration);
        Session? stripeSession = ev as Session;

        if (stripeSession == null)
        {
            return new ResultError(
                ResultErrorType.INVALID_OPERATION_ERROR,
                "Trying to run checkout session completed strategy when given object is not a checkout session!"
            );
        }

        if (!stripeSession.Metadata.TryGetValue("userid", out string? userId) || string.IsNullOrWhiteSpace(userId))
        {
            return new ResultError(
                ResultErrorType.INVALID_OPERATION_ERROR,
                "Checkout session does not have a userid attached to its metadata!"
            );
        }


        if (!stripeSession.Metadata.TryGetValue("orderid", out string? orderId) || string.IsNullOrWhiteSpace(orderId))
        {
            return new ResultError(
                ResultErrorType.INVALID_OPERATION_ERROR,
                "Checkout session does not have a orderid attached to its metadata!"
            );
        }

        long amount = stripeSession.AmountTotal ?? throw new InvalidOperationException("Amount total is null");

        var kafkaEvent = new CheckoutSucceededEvent
        {
            UserId = userId,
            OrderId = orderId,
            Amount = amount,
        };

        string jsonMessage = JsonSerializer.Serialize(kafkaEvent);

        await producer.ProduceEventAsync(kafkaEvent.TopicName, jsonMessage, CancellationToken.None);
        return null;
    }
}
