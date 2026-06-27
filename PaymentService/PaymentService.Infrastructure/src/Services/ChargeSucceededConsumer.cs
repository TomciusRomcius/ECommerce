using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.src.Interfaces;
using PaymentService.Domain.src.Utils;
using PaymentService.Infrastructure.Interfaces;

namespace PaymentService.Infrastructure.Services;

public class ChargeSucceededConsumer : IChargeSucceededConsumer
{
    private readonly ILogger<ChargeSucceededConsumer> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IPaymentSessionPersistenceService _paymentSessionPersistenceService;
    private const string TopicName = "checkout_succeeded";

    public ChargeSucceededConsumer(
        ILogger<ChargeSucceededConsumer> logger,
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IPaymentSessionPersistenceService paymentSessionPersistenceService
    )
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
        _paymentSessionPersistenceService = paymentSessionPersistenceService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                KafkaEventConsumer consumer = new(_kafkaConfiguration,
                    AutoOffsetReset.Latest,
                    "payment-service",
                    TopicName);

                CheckoutSucceededEvent? ev = consumer.Consume<CheckoutSucceededEvent>(cancellationToken);
                if (ev is null)
                {
                    _logger.LogError("Failed to parse the charge succeeded event!");
                    continue;
                }

                ResultError? error = await _paymentSessionPersistenceService.DeleteAsync(new Guid(ev.UserId));
                if (error is not null)
                {
                    _logger.LogError("Failed to delete the payment session of the user. Error: {@Error}", error);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse the charge succeeded event! Exception: {@Exception}", ex);
            }
        }
    }
}
