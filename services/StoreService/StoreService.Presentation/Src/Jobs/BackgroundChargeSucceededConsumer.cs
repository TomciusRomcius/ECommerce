using StoreService.Application.Interfaces;
using StoreService.Application.Services;

namespace StoreService.Presentation.Jobs;

public class BackgroundChargeSucceededConsumer : BackgroundService
{
    private readonly ILogger<BackgroundChargeSucceededConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundChargeSucceededConsumer(ILogger<BackgroundChargeSucceededConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IChargeSucceededConsumer chargeSucceededConsumer = scope.ServiceProvider.GetRequiredService<IChargeSucceededConsumer>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await chargeSucceededConsumer.TryConsumeAndHandle(stoppingToken);
                }
            },
            stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
