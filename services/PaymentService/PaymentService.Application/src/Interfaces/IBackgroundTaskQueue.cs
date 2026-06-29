namespace PaymentService.Application.src.Interfaces;

public interface IBackgroundTaskQueue
{
    public Task QueueBackgroundWorkItemAsync(
        Func<CancellationToken, Task> func
    );

    public Task<Func<CancellationToken, Task>> DequeueBackgroundWorkItemAsync(
        CancellationToken cancellationToken
    );
}