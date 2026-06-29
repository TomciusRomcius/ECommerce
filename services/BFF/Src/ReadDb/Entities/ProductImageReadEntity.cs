namespace BFF.ReadDb.Entities;

public sealed class ProductImageReadEntity
{
    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// AWS S3 object key.
    /// </summary>
    public string S3Key { get; set; } = string.Empty;
}
