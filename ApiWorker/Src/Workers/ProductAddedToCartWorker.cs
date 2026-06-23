using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductAddedToCartWorker(
    ILogger<ProductAddedToCartWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductAddedToCartEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product-added-to-cart";

    protected override string EventLabel => "Product-added-to-cart";
}
