using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker;

public class Worker(ILogger<Worker> logger, IOptions<KafkaConfiguration> kafkaConfiguration) : BackgroundService
{
    private readonly string _groupId = "api-worker";
    private readonly string _topic = "product-added-to-store";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        KafkaEventConsumer consumer = new KafkaEventConsumer(kafkaConfiguration.Value, AutoOffsetReset.Earliest, _groupId, _topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            ProductAddedToStoreEvent? ev = consumer.Consume<ProductAddedToStoreEvent>(stoppingToken);
            if (ev == null)
            {
                logger.LogError("Failed to consume event {Topic}", _topic);
                break;
            }
            
            

            await Task.Delay(1000, stoppingToken);
        }
    }
}