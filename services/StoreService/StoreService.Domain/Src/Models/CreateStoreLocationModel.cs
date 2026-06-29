namespace StoreService.Domain.Models;

public class CreateStoreLocationModel
{
    public CreateStoreLocationModel(string displayName, string address)
    {
        DisplayName = displayName;
        Address = address;
    }

    public string DisplayName { get; set; }
    public string Address { get; set; }
}