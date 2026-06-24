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
using StoreService.Domain.Utils;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

public class AddProductToStoreHandler(
    ILogger<AddProductToStoreHandler> logger,
    DatabaseContext context,
    IOptions<KafkaConfiguration> kafkaConfiguration) : IRequestHandler<AddProductToStoreCommand, ResultError?>
{
    public async Task<ResultError?> Handle(AddProductToStoreCommand request, CancellationToken cancellationToken)
    {
        logger.LogTrace("Entered Handle");
        logger.LogDebug(
            "Adding product with id: {ProductId} to store with id: {StoreLocationId}",
            request.ProductStoreLocation.ProductId,
            request.ProductStoreLocation.StoreLocationId
        );

        bool exists = await context.ProductStoreLocations
            .AnyAsync(psl => psl.ProductId == request.ProductStoreLocation.ProductId 
                && psl.StoreLocationId == request.ProductStoreLocation.StoreLocationId);

        if (exists)
        {
            return new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "The product is already in the store.");
        }

        logger.LogDebug("Product store location entity: {@ProductStoreLocation}", request.ProductStoreLocation);
        await context.ProductStoreLocations.AddAsync(request.ProductStoreLocation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Successfully added product with id: {ProductId} to the store",
            request.ProductStoreLocation.ProductId
        );

        var kafkaEvent = new ProductAddedToStoreEvent
        {
            ProductId = request.ProductStoreLocation.ProductId,
            StoreLocationId = request.ProductStoreLocation.StoreLocationId,
            Stock = request.ProductStoreLocation.Stock,
        };
        string sEvent = JsonUtils.Serialize(kafkaEvent);
        await new KafkaEventProducer(kafkaConfiguration.Value)
            .ProduceEventAsync("product-added-to-store", sEvent, cancellationToken);

        return null;
    }
}
