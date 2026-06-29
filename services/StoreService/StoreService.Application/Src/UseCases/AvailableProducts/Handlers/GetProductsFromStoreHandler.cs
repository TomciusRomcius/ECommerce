using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.AvailableProducts.Queries;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.AvailableProducts.Handlers;

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

        _logger.LogDebug(
            "Retrieved {Count} products for store {StoreLocationId}",
            page.Data.Count,
            request.StoreLocationId);

        return page;
    }
}
