using PaymentService.Infrastructure.Interfaces;

namespace PaymentService.Presentation.BackgroundServices;

public class ChargeSucceededBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var scope = serviceProvider.CreateScope();
            IChargeSucceededConsumer consumer = scope.ServiceProvider.GetRequiredService<IChargeSucceededConsumer>();
            await consumer.StartAsync(stoppingToken);
        }, stoppingToken);
    }
}
