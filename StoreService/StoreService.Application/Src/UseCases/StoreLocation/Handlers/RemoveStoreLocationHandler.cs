using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.StoreLocation.Commands;

namespace StoreService.Application.UseCases.StoreLocation.Handlers;

public class RemoveStoreLocationHandler : IRequestHandler<RemoveStoreLocationCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<RemoveStoreLocationHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public RemoveStoreLocationHandler(
        ILogger<RemoveStoreLocationHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(RemoveStoreLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogInformation("Removing store location: {StoreLocationId}", request.StoreLocationId);

        var rowsDeleted = await _context.StoreLocations
            .Where(sl => sl.StoreLocationId == request.StoreLocationId)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
        {
            _logger.LogWarning("No store location found with id: {StoreLocationId}", request.StoreLocationId);
            return;
        }

        _logger.LogInformation("Deleted rows: {DeletedRows}", rowsDeleted);

        var ev = new StoreDeletedEvent
        {
            StoreLocationId = request.StoreLocationId,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync("store-deleted", sEvent, cancellationToken);
    }
}
