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

public interface ICheckoutSucceededEventListener
{
    Task StartAsync(CancellationToken cancellationToken);
}

public class ChargeSucceededBackgroundService : ICheckoutSucceededEventListener
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
            "checkout_succeeded");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                CheckoutSucceededEvent? chargeEvent = consumer.Consume<CheckoutSucceededEvent>(cancellationToken);
                if (chargeEvent == null)
                {
                    _logger.LogError("Failed to parse event type {EventName}!", nameof(CheckoutSucceededEvent));
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
                    nameof(CheckoutSucceededEvent));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unknown exception while consuming event type {EventName}",
                    nameof(CheckoutSucceededEvent));
            }
        }
    }
}
