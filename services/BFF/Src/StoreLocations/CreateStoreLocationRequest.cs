using System.ComponentModel.DataAnnotations;

namespace BFF.StoreLocations;

public class CreateStoreLocationRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string DisplayName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Address { get; set; }
}
