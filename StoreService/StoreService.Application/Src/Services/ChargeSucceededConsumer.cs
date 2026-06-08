using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreService.Application.Interfaces;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Domain.Entities;

namespace StoreService.Application.Services;

public class ChargeSucceededConsumer : IChargeSucceededConsumer
{
    private readonly ILogger<ChargeSucceededConsumer> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IOrderDetailsService _orderDetailsService;
    private readonly IMediator _mediator;

    public ChargeSucceededConsumer(ILogger<ChargeSucceededConsumer> logger,
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IOrderDetailsService orderDetailsService,
        IMediator mediator)
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
        _orderDetailsService = orderDetailsService;
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
                ChargeSucceededEvent? chargeEvent = consumer.Consume<ChargeSucceededEvent>(cancellationToken);

                if (chargeEvent == null)
                {
                    return;
                }

                GetOrdersResponseType response = await _orderDetailsService.FetchAsync(chargeEvent.UserId, chargeEvent.OrderId, cancellationToken);
                List<ProductStoreLocationEntity> updater = response.OrderProducts
                    .Select(x => new ProductStoreLocationEntity(x.StoreLocationId, x.ProductId, x.Quantity))
                    .ToList();

                await _mediator.Send(new UpdateProductsStockCommand(updater), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to consume or handle charge succeeded event. Exception: {Exception}", ex);
            }
        }
    }
}
