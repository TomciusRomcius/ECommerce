using UserService.Application.Services;

namespace UserService.Presentation.Background;

public class ChargeSucceededBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ChargeSucceededBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();

        // The service currently has blocking IO, so a separate thread is required.
        await Task.Run(async () =>
        {
            await scope.ServiceProvider
                .GetRequiredService<ICheckoutSucceededEventListener>()
                .StartAsync(stoppingToken);
        }, stoppingToken);
    }
}
