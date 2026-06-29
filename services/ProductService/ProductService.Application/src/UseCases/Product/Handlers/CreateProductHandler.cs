using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Commands;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;

namespace ProductService.Application.UseCases.Product.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductEntity>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public CreateProductHandler(
        ILogger<CreateProductHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _context = context;
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<Result<ProductEntity>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var productEntity = new ProductEntity(request.Name, request.Description, request.Price, request.ManufacturerId,
            request.CategoryId);

        _logger.LogDebug("Creating product: {@Product}", productEntity);

        await _context.Products.AddAsync(productEntity, cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            var ev = new ProductCreatedEvent
            {
                ProductId = productEntity.ProductId,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                ManufacturerId = productEntity.ManufacturerId,
                CategoryId = productEntity.CategoryId,
            };

            string sEvent = JsonUtils.Serialize(ev);
            await new KafkaEventProducer(_kafkaConfiguration)
                .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);

            IReadOnlyList<string> imageKeys = request.ImageKeys;
            if (imageKeys.Count == 0 && request.ImageCount > 0)
            {
                imageKeys = Enumerable.Range(0, request.ImageCount)
                    .Select(order => $"{productEntity.ProductId}_{order}")
                    .ToList();
            }

            List<ProductImageEntity> createdImages = [];
            foreach (string imageKey in imageKeys)
            {
                if (string.IsNullOrWhiteSpace(imageKey))
                {
                    continue;
                }

                var imageEntity = new ProductImageEntity(productEntity.ProductId, imageKey.Trim());
                createdImages.Add(imageEntity);
                await _context.ProductImages.AddAsync(imageEntity, cancellationToken);
            }

            if (createdImages.Count > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);

                KafkaEventProducer producer = new(_kafkaConfiguration);
                foreach (ProductImageEntity image in createdImages)
                {
                    var imageEv = new ProductImageCreatedEvent
                    {
                        ProductImageId = image.ProductImageId,
                        ProductId = image.ProductId,
                        S3Key = image.S3Key,
                    };

                    await producer.ProduceEventAsync(
                        imageEv.TopicName,
                        JsonUtils.Serialize(imageEv),
                        cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception was thrown while saving changes");
            return new Result<ProductEntity>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to create the product")
            ]);
        }

        _logger.LogInformation(
            "Created product. Name: {Name}, ProductId: {ProductId}",
            productEntity.Name,
            productEntity.ProductId
        );
        return new Result<ProductEntity>(productEntity);
    }
}
