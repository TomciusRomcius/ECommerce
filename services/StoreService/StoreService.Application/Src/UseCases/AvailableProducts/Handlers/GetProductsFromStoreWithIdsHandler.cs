using AutoMapper;
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
    private readonly IMapper _mapper;

    public GetProductsFromStoreWithIdsHandler(
        ILogger<GetProductsFromStoreWithIdsHandler> logger,
        DatabaseContext context,
        IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
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

        List<ProductStoreLocationQueryRow> rows = await _context.ProductStoreLocations
            .AsNoTracking()
            .Where(psl => request.ProductIds.Contains(psl.ProductId))
            .Join(
                _context.StoreLocations.AsNoTracking(),
                psl => psl.StoreLocationId,
                store => store.StoreLocationId,
                (psl, store) => new ProductStoreLocationQueryRow
                {
                    ProductId = psl.ProductId,
                    StoreLocationId = psl.StoreLocationId,
                    Stock = psl.Stock,
                    DisplayName = store.DisplayName,
                    Address = store.Address,
                })
            .ToListAsync(cancellationToken);

        List<ProductStoreLocationDetails> result = _mapper.Map<List<ProductStoreLocationDetails>>(rows);

        _logger.LogDebug("Retrieved product store locations: {@Result}", result);
        return result;
    }
}
