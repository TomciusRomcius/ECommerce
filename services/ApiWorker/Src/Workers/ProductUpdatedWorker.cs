using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductUpdatedWorker(
    ILogger<ProductUpdatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductUpdatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_updated";

}
