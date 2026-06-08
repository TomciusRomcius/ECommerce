using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.AvailableProducts.Queries;

namespace StoreService.Application.UseCases.AvailableProducts.Handlers;

public class GetProductsFromStoreWithIdsHandler
    : IRequestHandler<GetProductsFromStoreWithIdsQuery, List<ProductStoreLocationDetails>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetProductsFromStoreWithIdsHandler> _logger;

    public GetProductsFromStoreWithIdsHandler(
        ILogger<GetProductsFromStoreWithIdsHandler> logger,
        DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<ProductStoreLocationDetails>> Handle(
        GetProductsFromStoreWithIdsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");

        if (request.ProductIds.Count == 0)
        {
            return [];
        }

        var reservedCounts = _context.ProductStoreLocations
            .AsNoTracking()
            .Where(psl => request.ProductIds.Contains(psl.ProductId))
            .Select(psl => new
            {
                psl.StoreLocationId,
                psl.ProductId,
                Reserved = _context.ReservedProducts
                    .Where(rp => rp.StoreLocationId == psl.StoreLocationId && rp.ProductId == psl.ProductId)
                    .Sum(rp => (int?)rp.Stock) ?? 0,
            });

        List<ProductStoreLocationDetails> result = await _context.ProductStoreLocations
            .AsNoTracking()
            .Where(psl => request.ProductIds.Contains(psl.ProductId))
            .Join(
                _context.StoreLocations.AsNoTracking(),
                psl => psl.StoreLocationId,
                store => store.StoreLocationId,
                (psl, store) => new { psl, store })
            .Join(
                reservedCounts,
                row => new { row.psl.StoreLocationId, row.psl.ProductId },
                reserved => new { reserved.StoreLocationId, reserved.ProductId },
                (row, reserved) => new ProductStoreLocationDetails
                {
                    ProductId = row.psl.ProductId,
                    StoreLocationId = row.psl.StoreLocationId,
                    Stock = Math.Max(0, row.psl.Stock - reserved.Reserved),
                    DisplayName = row.store.DisplayName,
                    Address = row.store.Address,
                })
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved product store locations: {@Result}", result);
        return result;
    }
}
