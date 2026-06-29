using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Queries;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;

namespace ProductService.Application.UseCases.Product.Handlers;

public class GetProductsByIdHandler : IRequestHandler<GetProductsByIdQuery, Result<List<ProductEntity>>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetProductsByIdHandler> _logger;

    public GetProductsByIdHandler(ILogger<GetProductsByIdHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<ProductEntity>>> Handle(
        GetProductsByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");

        if (request.ProductIds.Count == 0)
        {
            return new Result<List<ProductEntity>>([
                new ResultError(ResultErrorType.VALIDATION_ERROR, "At least one product id is required."),
            ]);
        }

        List<ProductEntity> products = await _context.Products
            .AsNoTracking()
            .Where(p => request.ProductIds.Contains(p.ProductId))
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved products: {@Products}", products);
        return new Result<List<ProductEntity>>(products);
    }
}
