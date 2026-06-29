using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductDeletedWorker(
    ILogger<ProductDeletedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductDeletedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_deleted";

}
