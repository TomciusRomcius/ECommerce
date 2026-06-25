using Confluent.Kafka;
using ECommerceBackend.EventTypes;
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
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                KafkaEventConsumer consumer = new(
                    _kafkaConfiguration.Value,
                    AutoOffsetReset.Earliest,
                    "user-service",
                    "charge_succeeded"
                );

                ChargeSucceededEvent? chargeEvent = consumer.Consume<ChargeSucceededEvent>(cancellationToken);
                if (chargeEvent == null)
                {
                    _logger.LogError("Failed to parse event type {EventName}!", nameof(ChargeSucceededEvent));
                    continue;
                }

                await _dbContext.CartProducts
                    .Where(cp => cp.UserId == chargeEvent.UserId)
                    .ExecuteDeleteAsync(cancellationToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(
                    "Failed to parse event type {EventName}! Exception: {@Ex}",
                    nameof(ChargeSucceededEvent),
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unknown exception while consuming event type {EventName}. Exception: {@Ex}",
                    nameof(ChargeSucceededEvent),
                    ex
                );
            }
        }
    }
}
