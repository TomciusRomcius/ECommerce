using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

public class RemoveProductFromStoreHandler : IRequestHandler<RemoveProductFromStoreCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<RemoveProductFromStoreHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public RemoveProductFromStoreHandler(
        ILogger<RemoveProductFromStoreHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(RemoveProductFromStoreCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var rowsAffected = await _context.ProductStoreLocations
            .Where(psl => psl.StoreLocationId == request.StoreLocationId && psl.ProductId == request.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        var ev = new ProductRemovedFromStoreEvent()
        {
            ProductId = request.ProductId,
            StoreLocationId = request.StoreLocationId
        };

        if (rowsAffected > 0)
        {
            _logger.LogInformation(
                "Removed product(id: {ProductId}) from store location(id: {StoreLocationId})",
                request.ProductId,
                request.StoreLocationId
            );

            string sEvent = JsonUtils.Serialize(ev);
            await new KafkaEventProducer(_kafkaConfiguration).ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
        }
        else
        {
            _logger.LogWarning(@"Failed to remove product(id: {ProductId}) from store location(id: {StoreLocationId})
                                because product or store location does not exist",
                request.ProductId,
                request.StoreLocationId
            );
        }
    }
}