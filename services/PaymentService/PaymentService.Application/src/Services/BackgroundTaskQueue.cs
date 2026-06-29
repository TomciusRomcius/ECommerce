using System.Threading.Channels;
using PaymentService.Application.src.Interfaces;

namespace PaymentService.Application.src.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public BackgroundTaskQueue()
    {
        var options = new BoundedChannelOptions(100);
        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    public async Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        await _queue.Writer.WriteAsync(func);
    }

    public async Task<Func<CancellationToken, Task>> DequeueBackgroundWorkItemAsync(
        CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync();
    }
}