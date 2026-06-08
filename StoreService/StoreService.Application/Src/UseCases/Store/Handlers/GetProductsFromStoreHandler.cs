using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.Store.Queries;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.Store.Handlers;

public class GetProductsFromStoreHandler
    : IRequestHandler<GetProductsFromStoreQuery, Page<ProductStoreLocationEntity>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetProductsFromStoreHandler> _logger;

    public GetProductsFromStoreHandler(
        ILogger<GetProductsFromStoreHandler> logger,
        DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Page<ProductStoreLocationEntity>> Handle(
        GetProductsFromStoreQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug(
            "Fetching products for store {StoreLocationId}, page {PageNumber}, size {PageSize}",
            request.StoreLocationId,
            request.PageNumber,
            request.PageSize);

        Page<ProductStoreLocationEntity> page = await _context.ProductStoreLocations
            .AsNoTracking()
            .Where(psl => psl.StoreLocationId == request.StoreLocationId)
            .OrderBy(psl => psl.ProductId)
            .ToPageAsync(request.PageNumber, request.PageSize);

        if (page.Data.Count > 0)
        {
            List<int> productIds = page.Data.Select(psl => psl.ProductId).ToList();

            Dictionary<int, int> reservedByProductId = await _context.ReservedProducts
                .AsNoTracking()
                .Where(rp => rp.StoreLocationId == request.StoreLocationId && productIds.Contains(rp.ProductId))
                .GroupBy(rp => rp.ProductId)
                .Select(group => new { ProductId = group.Key, Reserved = group.Sum(rp => rp.Stock) })
                .ToDictionaryAsync(group => group.ProductId, group => group.Reserved, cancellationToken);

            foreach (ProductStoreLocationEntity product in page.Data)
            {
                int reserved = reservedByProductId.GetValueOrDefault(product.ProductId, 0);
                product.Stock = Math.Max(0, product.Stock - reserved);
            }
        }

        _logger.LogDebug(
            "Retrieved {Count} products for store {StoreLocationId}",
            page.Data.Count,
            request.StoreLocationId);

        return page;
    }
}
