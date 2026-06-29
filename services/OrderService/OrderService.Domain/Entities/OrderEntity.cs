using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities;

public enum OrderState
{
    Active = 0,
    Finalized = 1
}

public class OrderEntity
{
    public required Guid OrderEntityId { get; set; }
    [Required]
    public required Guid UserId { get; set; }
    [Required]
    public IEnumerable<OrderProductEntity>? OrderProducts { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderState OrderState { get; set; }
}
