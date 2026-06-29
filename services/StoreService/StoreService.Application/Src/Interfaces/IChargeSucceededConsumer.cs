namespace StoreService.Application.Interfaces;

public interface IChargeSucceededConsumer
{
    Task TryConsumeAndHandle(CancellationToken cancellationToken);
}
