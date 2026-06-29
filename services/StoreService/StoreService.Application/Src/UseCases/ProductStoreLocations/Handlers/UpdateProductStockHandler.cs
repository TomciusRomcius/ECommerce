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

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateProductStockHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public UpdateProductStockHandler(ILogger<UpdateProductStockHandler> logger, DatabaseContext context, IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _context = context;
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Updating product. Updator: {@Updator}", request.ProductStoreLocation);
        var rowsAffected = await _context.ProductStoreLocations
            .Where(psl => psl.StoreLocationId == request.ProductStoreLocation.StoreLocationId
                          && psl.ProductId == request.ProductStoreLocation.ProductId
            )
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(psl => psl.Stock, request.ProductStoreLocation.Stock
                ));

        var ev = new ProductStockUpdatedEvent()
        {
            ProductId = request.ProductStoreLocation.ProductId,
            StoreLocationId = request.ProductStoreLocation.StoreLocationId,
            Stock = request.ProductStoreLocation.Stock
        };


        if (rowsAffected > 0)
        {
            _logger.LogInformation(
                "Updated product(id: {ProductId}) in store location(id: {StoreLocationId})",
                request.ProductStoreLocation.ProductId,
                request.ProductStoreLocation.StoreLocationId
            );

            string sEvent = JsonUtils.Serialize(ev);
            await new KafkaEventProducer(_kafkaConfiguration).ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
        }
        else
        {
            _logger.LogWarning(@"Failed to update product(id: {ProductId}) in store location(id: {StoreLocationId})
                                because product or store location does not exist",
                request.ProductStoreLocation.ProductId,
                request.ProductStoreLocation.StoreLocationId
            );
        }
    }
}