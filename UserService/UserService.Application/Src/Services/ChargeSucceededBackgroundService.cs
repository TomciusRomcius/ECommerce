using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Persistence;

namespace UserService.Application.Services;

public interface IChargeSucceededEventListener
{
    Task StartAsync(CancellationToken cancellationToken);
}

public class ChargeSucceededBackgroundService : IChargeSucceededEventListener
{
    private readonly ILogger<ChargeSucceededBackgroundService> _logger;
    private readonly DatabaseContext _dbContext;
    private readonly IOptions<KafkaConfiguration> _kafkaConfiguration;
    private readonly IServiceProvider _serviceProvider;

    public ChargeSucceededBackgroundService(ILogger<ChargeSucceededBackgroundService> logger,
        DatabaseContext dbContext,
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _dbContext = dbContext;
        _kafkaConfiguration = kafkaConfiguration;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started {ListenerName}", nameof(ChargeSucceededBackgroundService));
        var consumer = new KafkaEventConsumer(
            _kafkaConfiguration.Value,
            AutoOffsetReset.Earliest,
            "user-service",
            "charge_succeeded"
        );

        var producer = new KafkaEventProducer(_kafkaConfiguration.Value);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                ChargeSucceededEvent? chargeEvent = consumer.Consume<ChargeSucceededEvent>(cancellationToken);
                if (chargeEvent == null)
                {
                    _logger.LogError("Failed to parse event type {EventName}!", nameof(ChargeSucceededEvent));
                    continue;
                }
                _logger.LogDebug("Handling event {TopicName}", chargeEvent.TopicName);

                await _dbContext.CartProducts
                    .Where(cp => cp.UserId == chargeEvent.UserId)
                    .ExecuteDeleteAsync(cancellationToken);

                var kafkaEvent = new UserCartClearedEvent
                {
                    UserId = chargeEvent.UserId
                };

                await producer.ProduceEventAsync(kafkaEvent.TopicName, 
                    JsonUtils.Serialize(kafkaEvent),
                    cancellationToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to parse event type {EventName}! StackTrace: {StackTrace}",
                    nameof(ChargeSucceededEvent),
                    ex.StackTrace
                );
            }
            catch (Exception ex)
            {
             _logger.LogError(
                    ex,
                    "Failed to parse event type {EventName}! StackTrace: {StackTrace}",
                    nameof(ChargeSucceededEvent),
                    ex.StackTrace
                );
            }
        }
    }
}
