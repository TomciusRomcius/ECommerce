using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Queries;
using ProductService.Domain.Entities;

namespace ProductService.Application.UseCases.Product.Handlers;

public class GetProductByNameHandler : IRequestHandler<GetProductByNameQuery, ProductEntity?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetProductByNameHandler> _logger;

    public GetProductByNameHandler(ILogger<GetProductByNameHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ProductEntity?> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Searching for product: {Name}", request.Name);
        var product = await _context.Products.Where(p => p.Name == request.Name).FirstOrDefaultAsync();
        _logger.LogDebug("Retrieved product: {@Product}", product);
        return product;
    }
}