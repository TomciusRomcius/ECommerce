using System.ComponentModel.DataAnnotations;

namespace BFF.ReadDb.Entities;

public sealed class StoreLocationEntity
{
    public int StoreLocationId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
