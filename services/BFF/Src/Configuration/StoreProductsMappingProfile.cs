using AutoMapper;
using BFF.ReadDb.Entities;
using BFF.StoreLocations;
using BFF.StoreProducts;

namespace BFF.Configuration;

public class StoreProductsMappingProfile : Profile
{
    public StoreProductsMappingProfile()
    {
        CreateMap<StoreLocationEntity, StoreLocationDto>();

        CreateMap<StoreProductReadRow, StoreProductDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.StoreProduct.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product != null ? src.Product.Description : string.Empty))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0m))
            .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.Product != null ? src.Product.ManufacturerId : 0))
            .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer != null ? src.Manufacturer.Name : string.Empty))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product != null ? src.Product.CategoryId : 0))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.StoreLocation))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StoreProduct.Stock))
            .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
                .AfterMap<StoreProductImageUrlsMappingAction>();
    }
}
