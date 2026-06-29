using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Utils;

public class MicroserviceNetworkConfig
{
    [Required]
    public required string PaymentServiceUrl { get; set; }

    [Required]
    public required string ProductServiceUrl { get; set; }

    [Required]
    public required string UserServiceUrl { get; set; }

    [Required]
    public required string StoreServiceUrl { get; set; }
}
