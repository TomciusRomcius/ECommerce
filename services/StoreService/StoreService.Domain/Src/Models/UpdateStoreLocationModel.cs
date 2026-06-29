namespace StoreService.Domain.Models;

public class UpdateStoreLocationModel
{
    public UpdateStoreLocationModel(int storeLocationId, string? displayName, string? address)
    {
        StoreLocationId = storeLocationId;
        DisplayName = displayName;
        Address = address;
    }

    public int StoreLocationId { get; set; }
    public string? DisplayName { get; set; }
    public string? Address { get; set; }
}