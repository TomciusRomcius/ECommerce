using System.Text.Json;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
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

        logger.LogDebug("Product store location entity: {@ProductStoreLocation}", request.ProductStoreLocation);
        await context.ProductStoreLocations.AddAsync(request.ProductStoreLocation, cancellationToken);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Successfully added product with id: {ProductId} to the store",
                request.ProductStoreLocation.ProductId
            );
            
            var producer = new KafkaEventProducer(kafkaConfiguration.Value);
            var kafkaEvent = new ProductAddedToStoreEvent()
            {
                ProductId = request.ProductStoreLocation.ProductId,
                StoreLocationId = request.ProductStoreLocation.StoreLocationId,
                Stock = request.ProductStoreLocation.Stock,
            };
            string kafkaEventJson = JsonSerializer.Serialize(kafkaEvent);
            await producer.ProduceEventAsync("product-added-to-store", kafkaEventJson, cancellationToken);
            
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encountered an exception while adding a product to a store");
            return new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to add the product to the store");
        }
    }
}