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

public class UpdateStoreLocationHandler : IRequestHandler<UpdateStoreLocationCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateStoreLocationHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public UpdateStoreLocationHandler(
        ILogger<UpdateStoreLocationHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(UpdateStoreLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Updating store location with updator: {@Updator}", request.Updator);
        var updator = request.Updator;

        var rowsAffected = await _context.StoreLocations
            .Where(sl => sl.StoreLocationId == request.Updator.StoreLocationId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(sl => sl.Address, updator.Address)
                    .SetProperty(sl => sl.DisplayName, updator.DisplayName)
            );

        if (rowsAffected > 0)
        {
            _logger.LogInformation("Succesfully updated store location");

            var ev = new StoreUpdatedEvent
            {
                StoreLocationId = updator.StoreLocationId,
                DisplayName = updator.DisplayName,
                Address = updator.Address,
            };

            string sEvent = JsonUtils.Serialize(ev);
            await new KafkaEventProducer(_kafkaConfiguration)
                .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
        }
        else
        {
            _logger.LogWarning(
                "Failed to update store location: location with id: {Id} does not exist",
                updator.StoreLocationId
            );
        }
    }
}
