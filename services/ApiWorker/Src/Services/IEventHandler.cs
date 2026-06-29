using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public interface IEventHandler<in TEvent> where TEvent : Event
{
    Task HandleAsync(TEvent ev, CancellationToken cancellationToken);
}
