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
            try
            {
                ProductAddedToStoreEvent? ev = consumer.Consume<ProductAddedToStoreEvent>(stoppingToken);
                if (ev == null)
                {
                    logger.LogError("Failed to consume event {Topic}", _topic);
                    break;
                }

                using IServiceScope scope = serviceScopeFactory.CreateScope();
                ReadDbContext readDbContext =
                    scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                IProductAddedToStoreHandler handler =
                    scope.ServiceProvider.GetRequiredService<IProductAddedToStoreHandler>();

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

                logger.LogInformation("Product-added-to-store event handled: {@Event}", ev);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process product-added-to-store event for product."
                );
            }
        }
    }
}
