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
using ProductService.Domain.Entities;

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

        List<ProductImageEntity> images = await _context.ProductImages
            .Where(image => image.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        await _context.ProductImages
            .Where(image => image.ProductId == request.ProductId)
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

        KafkaEventProducer producer = new(_kafkaConfiguration);
        foreach (ProductImageEntity image in images)
        {
            var imageEv = new ProductImageDeletedEvent
            {
                ProductImageId = image.ProductImageId,
                ProductId = image.ProductId,
            };

            await producer.ProduceEventAsync(
                "product-image-deleted",
                JsonUtils.Serialize(imageEv),
                cancellationToken);
        }

        var ev = new ProductDeletedEvent
        {
            ProductId = request.ProductId,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
    }
}
