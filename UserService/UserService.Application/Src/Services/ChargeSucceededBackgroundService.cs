using Confluent.Kafka;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.UseCases.Cart.Commands;

namespace UserService.Application.Services;

public interface IChargeSucceededEventListener
{
    Task StartAsync(CancellationToken cancellationToken);
}

public class ChargeSucceededBackgroundService : IChargeSucceededEventListener
{
    private readonly ILogger<ChargeSucceededBackgroundService> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ChargeSucceededBackgroundService(
        ILogger<ChargeSucceededBackgroundService> logger,
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started {ListenerName}", nameof(ChargeSucceededBackgroundService));

        KafkaEventConsumer consumer = new(
            _kafkaConfiguration,
            AutoOffsetReset.Earliest,
            "user-service",
            "charge_succeeded");

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

                _logger.LogDebug(
                    "Handling charge succeeded event for user {UserId}",
                    chargeEvent.UserId);

                await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new EraseUserCartCommand(new Guid(chargeEvent.UserId)), cancellationToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to consume event type {EventName}",
                    nameof(ChargeSucceededEvent));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unknown exception while consuming event type {EventName}",
                    nameof(ChargeSucceededEvent));
            }
        }
    }
}
