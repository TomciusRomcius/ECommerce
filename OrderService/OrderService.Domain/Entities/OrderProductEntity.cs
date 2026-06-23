using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities;

public class OrderProductEntity
{
    [Required]
    public required Guid OrderId { get; set; }
    [Required]
    public required int ProductId { get; set; }
    [Required]
    public required int StoreLocationId { get; set; }
    [Required]
    public required string ProductName { get; set; }
    [Required]
    [Range(0, Int32.MaxValue)]
    public required int Quantity { get; set; }
}
