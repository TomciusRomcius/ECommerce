using Newtonsoft.Json;

namespace StoreService.Application.Services;

public class GetOrdersResponseType
{
    public class OrderProduct
    {
        [JsonProperty(Required = Required.Always)]
        public required Guid OrderId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public required int StoreLocationId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public required int ProductId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public required string ProductName { get; set; }
        [JsonProperty(Required = Required.Always)]
        public required int Quantity { get; set; }
    }

    [JsonProperty(Required = Required.Always)]
    public Guid OrderEntityId { get; set; }
    [JsonProperty(Required = Required.Always)]
    public required Guid UserId { get; set; }
    public List<OrderProduct> OrderProducts { get; set; } = [];
    [JsonProperty(Required = Required.Always)]
    public DateTime CreatedAt { get; set; }
}

public interface IOrderDetailsService
{
    Task<GetOrdersResponseType> FetchAsync(string userId, string orderId, CancellationToken cancellationToken);
}
