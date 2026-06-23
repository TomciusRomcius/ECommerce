using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductCartQuantityModifiedWorker(
    ILogger<ProductCartQuantityModifiedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductCartQuantityModifiedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product-cart-quantity-modified";

    protected override string EventLabel => "Product-cart-quantity-modified";
}
