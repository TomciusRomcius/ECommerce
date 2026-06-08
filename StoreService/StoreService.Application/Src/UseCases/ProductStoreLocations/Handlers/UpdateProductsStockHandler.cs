using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

// TODO: known limitation: if two users finish the payment session the stock can become negative so this should be addressed.

public class UpdateProductsStockHandler : IRequestHandler<UpdateProductsStockCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateProductsStockHandler> _logger;

    public UpdateProductsStockHandler(ILogger<UpdateProductsStockHandler> logger, DatabaseContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdateProductsStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Updating products. Updator: {@Updator}", request.ProductStoreLocations);

        List<ProductStoreLocationEntity> req = request.ProductStoreLocations;
        
        ExpressionStarter<ProductStoreLocationEntity>? predicate = 
            PredicateBuilder.New<ProductStoreLocationEntity>(false);

        foreach (ProductStoreLocationEntity item in req)
        {
            int storeLocationId = item.StoreLocationId;
            int productId = item.ProductId;
    
            predicate = predicate.Or(psl => 
                psl.StoreLocationId == storeLocationId && 
                psl.ProductId == productId);
        }

        List<ProductStoreLocationEntity> results = await _context.ProductStoreLocations
            .AsExpandable()
            .Where(predicate)
            .ToListAsync(cancellationToken: cancellationToken);
        
        // Update stocks
        Dictionary<(int StoreLocationId, int ProductId), int> stocks = new();
        foreach (ProductStoreLocationEntity product in req)
        {
            stocks[(product.StoreLocationId, product.ProductId)] = product.Stock;
        }

        foreach (ProductStoreLocationEntity product in results)
        {
            product.Stock -= stocks[(StoreLocationId: product.StoreLocationId, ProductId: product.ProductId)];
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
