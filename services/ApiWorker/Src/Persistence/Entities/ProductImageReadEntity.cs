namespace ApiWorker.Persistence.Entities;

public sealed class ProductImageReadEntity
{
    private ProductImageReadEntity()
    {
        S3Key = null!;
    }

    public ProductImageReadEntity(int productImageId, int productId, string s3Key)
    {
        ProductImageId = productImageId;
        ProductId = productId;
        S3Key = s3Key;
    }

    public ProductImageReadEntity(int productId, string s3Key)
    {
        ProductId = productId;
        S3Key = s3Key;
    }

    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// AWS S3 object key.
    /// </summary>
    public string S3Key { get; set; }
}
