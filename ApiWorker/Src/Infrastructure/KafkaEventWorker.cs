using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ApiWorker.Services;
using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiWorker.Infrastructure;

public abstract class KafkaEventWorker<TEvent>(
    ILogger logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
    where TEvent : Event
{
    private const string GroupId = "api-worker";

    protected abstract string Topic { get; }

    protected abstract string EventLabel { get; }

    protected virtual async Task HandleEventAsync(
        TEvent ev,
        IServiceScope scope,
        CancellationToken cancellationToken)
    {
        IEventHandler<TEvent> handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
        await handler.HandleAsync(ev, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        KafkaEventConsumer consumer = new(
            kafkaConfiguration.Value,
            AutoOffsetReset.Earliest,
            GroupId,
            Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TEvent? ev = consumer.Consume<TEvent>(stoppingToken);
                if (ev == null)
                {
                    logger.LogError("Failed to consume event {Topic}", Topic);
                    break;
                }

                using IServiceScope scope = serviceScopeFactory.CreateScope();
                ReadDbContext readDbContext = scope.ServiceProvider.GetRequiredService<ReadDbContext>();

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

                await HandleEventAsync(ev, scope, stoppingToken);

                readDbContext.ProcessedMessages.Add(new ProcessedMessageEntity
                {
                    MessageId = messageId,
                });
                await readDbContext.SaveChangesAsync(stoppingToken);

                logger.LogInformation("{EventLabel} event handled: {@Event}", EventLabel, ev);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process {EventLabel} event.", EventLabel);
            }
        }
    }
}
