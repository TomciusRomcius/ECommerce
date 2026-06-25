using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductImageDeletedWorker(
    ILogger<ProductImageDeletedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductImageDeletedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_image_deleted";

}
