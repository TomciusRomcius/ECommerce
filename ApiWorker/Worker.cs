using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;
using ApiWorker.Services;

namespace ApiWorker;

public class Worker(
    ILogger<Worker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly string _groupId = "api-worker";
    private readonly string _topic = "product-added-to-store";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        KafkaEventConsumer consumer = new KafkaEventConsumer(
            kafkaConfiguration.Value,
            AutoOffsetReset.Earliest,
            _groupId,
            _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ProductAddedToStoreEvent? ev = consumer.Consume<ProductAddedToStoreEvent>(stoppingToken);
            if (ev == null)
            {
                logger.LogError("Failed to consume event {Topic}", _topic);
                break;
            }

            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IProductAddedToStoreHandler handler =
                    scope.ServiceProvider.GetRequiredService<IProductAddedToStoreHandler>();

                await handler.HandleAsync(
                    ev.StoreLocationId,
                    ev.ProductId,
                    ev.Stock,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process product-added-to-store event for product {ProductId} at store location {StoreLocationId}",
                    ev.ProductId,
                    ev.StoreLocationId);
            }
        }
    }
}
