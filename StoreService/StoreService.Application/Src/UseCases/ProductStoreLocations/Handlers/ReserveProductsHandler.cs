using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Domain.Entities;
using StoreService.Domain.Utils;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

public class ReserveProductsHandler(
    ILogger<ReserveProductsHandler> logger,
    DatabaseContext context) : IRequestHandler<ReserveProductsCommand, ResultError?>
{
    public async Task<ResultError?> Handle(ReserveProductsCommand request, CancellationToken cancellationToken)
    {
        logger.LogTrace("Entered Handle");
        logger.LogDebug("Reserving products for order {OrderId}", request.OrderId);

        if (request.Products.Count == 0)
        {
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "No products to reserve.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        ExpressionStarter<ProductStoreLocationEntity> predicate =
            PredicateBuilder.New<ProductStoreLocationEntity>(false);

        foreach (ReserveProductItem item in request.Products)
        {
            predicate = predicate.Or(psl =>
                psl.StoreLocationId == item.StoreLocationId && psl.ProductId == item.ProductId);
        }

        Dictionary<(int StoreLocationId, int ProductId), ProductStoreLocationEntity> productStoreLocationByKey = await context.ProductStoreLocations
            .Where(predicate)
            .ToDictionaryAsync(psl => (psl.StoreLocationId, psl.ProductId), cancellationToken);

        foreach (ReserveProductItem item in request.Products)
        {
            if (!productStoreLocationByKey.TryGetValue((item.StoreLocationId, item.ProductId), out ProductStoreLocationEntity? _))
            {
                return new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "Product not found in store.");
            }

            ProductStoreLocationEntity productStoreLocation =
                productStoreLocationByKey[(item.StoreLocationId, item.ProductId)];

            if (productStoreLocation.Stock < item.Stock)
            {
                return new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "Insufficient stock.");
            }
        }

        foreach (ReserveProductItem item in request.Products)
        {
            ProductStoreLocationEntity productStoreLocation =
                productStoreLocationByKey[(item.StoreLocationId, item.ProductId)];

            productStoreLocation.Stock -= item.Stock;

            context.ReservedProducts.Add(
                new ReservedProductEntity(request.OrderId, item.StoreLocationId, item.ProductId, item.Stock),
            );
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Successfully reserved products for order {OrderId}", request.OrderId);
        return null;
    }
}
