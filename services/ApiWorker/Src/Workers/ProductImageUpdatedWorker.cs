using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductImageUpdatedWorker(
    ILogger<ProductImageUpdatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductImageUpdatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_image_updated";

}
