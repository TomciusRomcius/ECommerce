using AutoMapper;
using BFF.Configuration;
using BFF.ReadDb.Entities;

namespace BFF.StoreProducts;

public class StoreProductsMappingProfile : Profile
{
    public StoreProductsMappingProfile()
    {
        CreateMap<StoreProductReadEntity, StoreProductStoreDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.StoreDisplayName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.StoreAddress));

        CreateMap<StoreProductReadEntity, StoreProductDto>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom<StoreProductImageUrlsResolver>());
    }
}

public class StoreProductImageUrlsResolver(IS3ImageUrlBuilder s3ImageUrlBuilder)
    : IValueResolver<StoreProductReadEntity, StoreProductDto, List<string>>
{
    public List<string> Resolve(
        StoreProductReadEntity source,
        StoreProductDto destination,
        List<string> destMember,
        ResolutionContext context) =>
        s3ImageUrlBuilder.BuildUrls(source.ProductImages.Select(image => image.S3Key)).ToList();
}
