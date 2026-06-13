using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Commands;

namespace ProductService.Application.UseCases.Product.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public DeleteProductHandler(
        ILogger<DeleteProductHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _context = context;
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogInformation("Deleting product: {ProductId}", request.ProductId);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.ProductImages
            .Where(pi => pi.ProductId == request.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        var rowsDeleted = await _context.Products
            .Where(p => p.ProductId == request.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        if (rowsDeleted == 0)
        {
            _logger.LogWarning("No product found with id: {ProductId}", request.ProductId);
            return;
        }

        _logger.LogInformation("Deleted product with id: {ProductId}", request.ProductId);

        var ev = new ProductDeletedEvent
        {
            ProductId = request.ProductId,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync("product-deleted", sEvent, cancellationToken);
    }
}
