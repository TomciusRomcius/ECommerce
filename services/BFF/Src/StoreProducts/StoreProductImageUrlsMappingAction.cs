using AutoMapper;
using BFF.Configuration;

namespace BFF.StoreProducts;

public sealed class StoreProductImageUrlsMappingAction(IS3ImageUrlBuilder s3ImageUrlBuilder)
    : IMappingAction<StoreProductReadRow, StoreProductDto>
{
    public void Process(StoreProductReadRow source, StoreProductDto destination, ResolutionContext context)
    {
        destination.ImageUrls = s3ImageUrlBuilder
            .BuildUrls(source.Images.Select(image => image.S3Key))
            .ToList();
    }
}
