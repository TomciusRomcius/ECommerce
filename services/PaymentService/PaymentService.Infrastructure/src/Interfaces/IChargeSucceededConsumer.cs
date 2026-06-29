namespace PaymentService.Infrastructure.Interfaces;

public interface IChargeSucceededConsumer
{
    Task StartAsync(CancellationToken cancellationToken);
}
