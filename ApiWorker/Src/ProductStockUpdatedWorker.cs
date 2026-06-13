using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ApiWorker.Services;
using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiWorker;

public class ProductStockUpdatedWorker(
    ILogger<ProductStockUpdatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly string _groupId = "api-worker";
    private readonly string _topic = "product-stock-updated";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        KafkaEventConsumer consumer = new KafkaEventConsumer(
            kafkaConfiguration.Value,
            AutoOffsetReset.Earliest,
            _groupId,
            _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ProductStockUpdatedEvent? ev = consumer.Consume<ProductStockUpdatedEvent>(stoppingToken);
                if (ev == null)
                {
                    logger.LogError("Failed to consume event {Topic}", _topic);
                    break;
                }

                using IServiceScope scope = serviceScopeFactory.CreateScope();
                ReadDbContext readDbContext =
                    scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                IProductStockUpdatedHandler handler =
                    scope.ServiceProvider.GetRequiredService<IProductStockUpdatedHandler>();

                string messageId = ev.MessageId.ToString();
                bool alreadyProcessed = await readDbContext.ProcessedMessages
                    .AnyAsync(message => message.MessageId == messageId, stoppingToken);
                if (alreadyProcessed)
                {
                    logger.LogInformation(
                        "Skipping already processed message {MessageId}",
                        messageId);
                    continue;
                }

                await handler.HandleAsync(
                    ev.StoreLocationId,
                    ev.ProductId,
                    ev.Stock,
                    stoppingToken);

                readDbContext.ProcessedMessages.Add(new ProcessedMessageEntity
                {
                    MessageId = messageId,
                });
                await readDbContext.SaveChangesAsync(stoppingToken);

                logger.LogInformation("Product-stock-updated event handled: {@Event}", ev);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process product-stock-updated event.");
            }
        }
    }
}
