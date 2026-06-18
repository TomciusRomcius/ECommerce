using System.ComponentModel.DataAnnotations;

namespace ApiWorker.Persistence.Entities;

public sealed class StoreLocationEntity
{
    public StoreLocationEntity(int storeLocationId, string displayName, string address)
    {
        StoreLocationId = storeLocationId;
        DisplayName = displayName;
        Address = address;
    }

    public int StoreLocationId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
