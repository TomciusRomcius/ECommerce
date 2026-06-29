using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductStockUpdatedWorker(
    ILogger<ProductStockUpdatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductStockUpdatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_stock_updated";

}
