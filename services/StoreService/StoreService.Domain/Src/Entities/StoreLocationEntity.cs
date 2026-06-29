using System.ComponentModel.DataAnnotations;

namespace StoreService.Domain.Entities;

public class StoreLocationEntity
{
    public StoreLocationEntity(string displayName, string address)
    {
        DisplayName = displayName;
        Address = address;
    }

    public StoreLocationEntity(int storeLocationId, string displayName, string address)
    {
        StoreLocationId = storeLocationId;
        DisplayName = displayName;
        Address = address;
    }

    [Key] public int StoreLocationId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Display name cannot be empty")]
    public string DisplayName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Address cannot be empty")]
    public string Address { get; set; }
}