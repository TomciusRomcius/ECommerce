using AutoMapper;
using StoreService.Domain.Entities;
using StoreService.Domain.Models;
using StoreService.Presentation.Controllers.AvailableProducts.dtos;
using StoreService.Presentation.Controllers.StoreLocation.dtos;

namespace StoreService.Presentation.Mapping;

public class PresentationMappingProfile : Profile
{
    public PresentationMappingProfile()
    {
        CreateMap<ProductStoreLocationEntity, ProductStoreLocationItemDto>();

        CreateMap<AddProductToStoreDto, ProductStoreLocationEntity>()
            .ConstructUsing(src => new ProductStoreLocationEntity(
                src.StoreLocationId, src.ProductId, src.Stock));

        CreateMap<RequestCreateLocationDto, CreateStoreLocationModel>()
            .ConstructUsing(src => new CreateStoreLocationModel(src.DisplayName, src.Address));

        CreateMap<RequestModifyLocationDto, UpdateStoreLocationModel>()
            .ConstructUsing(src => new UpdateStoreLocationModel(
                src.StoreLocationId, src.DisplayName, src.Address));
    }
}
