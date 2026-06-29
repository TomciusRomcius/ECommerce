using AutoMapper;
using ProductService.Application.Services;
using ProductService.Application.UseCases.Product.Commands;
using ProductService.Domain.Entities;
using ProductService.Presentation.Controllers.Product.Dtos;

namespace ProductService.Presentation.Mapping;

public class PresentationMappingProfile : Profile
{
    public const string StoreDetailsByProductIdKey = "StoreDetailsByProductId";

    public PresentationMappingProfile()
    {
        CreateMap<ProductEntity, ProductDto>()
            .ForMember(
                dest => dest.ImageKeys,
                opt => opt.MapFrom(src => src.Images.Select(image => image.S3Key).ToList()));

        CreateMap<ProductStoreDetails, StoreDetailsDto>();

        CreateMap<ProductEntity, ProductWithStoreDto>()
            .ForMember(
                dest => dest.Store,
                opt => opt.MapFrom((src, _, _, context) =>
                {
                    if (!context.Items.TryGetValue(StoreDetailsByProductIdKey, out object? item)
                        || item is not IReadOnlyDictionary<int, ProductStoreDetails> storeDetailsByProductId
                        || !storeDetailsByProductId.TryGetValue(src.ProductId, out ProductStoreDetails? storeDetails))
                    {
                        return null;
                    }

                    return context.Mapper.Map<StoreDetailsDto>(storeDetails);
                }));

        CreateMap<RequestCreateProductDto, CreateProductCommand>();
    }
}
