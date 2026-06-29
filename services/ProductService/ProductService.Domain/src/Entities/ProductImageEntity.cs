namespace ProductService.Domain.Entities;

public class ProductImageEntity
{
    private ProductImageEntity()
    {
        S3Key = null!;
    }

    public ProductImageEntity(int productId, string s3Key)
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

    public ProductEntity? Product { get; set; }
}
