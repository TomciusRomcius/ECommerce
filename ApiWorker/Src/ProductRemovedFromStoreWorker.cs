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

public class ProductRemovedFromStoreWorker(
    ILogger<ProductRemovedFromStoreWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly string _groupId = "api-worker";
    private readonly string _topic = "product-removed-from-store";

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
                ProductRemovedFromStoreEvent? ev = consumer.Consume<ProductRemovedFromStoreEvent>(stoppingToken);
                if (ev == null)
                {
                    logger.LogError("Failed to consume event {Topic}", _topic);
                    break;
                }

                using IServiceScope scope = serviceScopeFactory.CreateScope();
                ReadDbContext readDbContext =
                    scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                IProductRemovedFromStoreHandler handler =
                    scope.ServiceProvider.GetRequiredService<IProductRemovedFromStoreHandler>();

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
                    stoppingToken);

                readDbContext.ProcessedMessages.Add(new ProcessedMessageEntity
                {
                    MessageId = messageId,
                });
                await readDbContext.SaveChangesAsync(stoppingToken);

                logger.LogInformation("Product-removed-from-store event handled: {@Event}", ev);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process product-removed-from-store event.");
            }
        }
    }
}
