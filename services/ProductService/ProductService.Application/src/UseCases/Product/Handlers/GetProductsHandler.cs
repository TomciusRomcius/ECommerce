using System.Linq.Expressions;
using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Queries;
using ProductService.Domain.Entities;

namespace ProductService.Application.UseCases.Product.Handlers;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Page<ProductEntity>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetProductsHandler> _logger;

    public GetProductsHandler(ILogger<GetProductsHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Page<ProductEntity>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered {FunctionName}", nameof(GetProductsHandler));
        _logger.LogDebug(
            "Fetching products, page number: {PageNumber} page size: {PageSize}",
            request.PageNumber,
            request.PageSize
        );

        IQueryable<ProductEntity> query = _context.Products.AsNoTracking();
        string searchText = request.searchText.Trim();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string pattern = $"%{searchText}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, pattern) ||
                (p.Category != null && EF.Functions.ILike(p.Category.Name, pattern)) ||
                (p.Manufacturer != null && EF.Functions.ILike(p.Manufacturer.Name, pattern)));
        }

        Page<ProductEntity> page = await query
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.Images)
            .ToPageAsync(request.PageNumber, request.PageSize);

        _logger.LogDebug("Retrieved products page: {@Page}", page);
        return page;
    }
}
