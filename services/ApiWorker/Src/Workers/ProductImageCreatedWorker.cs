using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductImageCreatedWorker(
    ILogger<ProductImageCreatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductImageCreatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_image_created";

}
