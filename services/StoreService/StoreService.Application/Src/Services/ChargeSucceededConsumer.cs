using AutoMapper;
using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreService.Application.Interfaces;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;

namespace StoreService.Application.Services;

public class ChargeSucceededConsumer : IChargeSucceededConsumer
{
    private readonly ILogger<ChargeSucceededConsumer> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IMediator _mediator;

    public ChargeSucceededConsumer(
        ILogger<ChargeSucceededConsumer> logger,
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IMediator mediator)
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
        _mediator = mediator;
    }

    public async Task TryConsumeAndHandle(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {ConsumerName}", nameof(ChargeSucceededConsumer));
        KafkaEventConsumer consumer = new(_kafkaConfiguration,
            AutoOffsetReset.Earliest,
            "store-service",
            "charge-succeeded"
        );

        while (true)
        {
            try
            {
                CheckoutSucceededEvent? chargeEvent = consumer.Consume<CheckoutSucceededEvent>(cancellationToken);

                if (chargeEvent == null)
                {
                    return;
                }

                await _mediator.Send(
                    new FinalizeReservationCommand(Guid.Parse(chargeEvent.OrderId)),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to consume or handle charge succeeded event. Exception: {Exception}", ex);
            }
        }
    }
}
