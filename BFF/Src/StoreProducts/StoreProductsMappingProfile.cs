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
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.Product.ManufacturerId))
            .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Product.Manufacturer!.Name))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category!.Name))
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
        s3ImageUrlBuilder.BuildUrls(source.Product.Images.Select(image => image.S3Key)).ToList();
}
