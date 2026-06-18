using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductAddedToStoreWorker(
    ILogger<ProductAddedToStoreWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductAddedToStoreEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product-added-to-store";

    protected override string EventLabel => "Product-added-to-store";
}
