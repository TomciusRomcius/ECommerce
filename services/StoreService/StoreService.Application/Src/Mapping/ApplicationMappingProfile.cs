using AutoMapper;
using StoreService.Application.Services;
using StoreService.Application.UseCases.AvailableProducts;
using StoreService.Application.UseCases.AvailableProducts.Queries;
using StoreService.Domain.Entities;
using StoreService.Domain.Models;

namespace StoreService.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CreateStoreLocationModel, StoreLocationEntity>()
            .ConstructUsing(src => new StoreLocationEntity(src.DisplayName, src.Address));

        CreateMap<GetOrdersResponseType.OrderProduct, ProductStoreLocationEntity>()
            .ConstructUsing(src => new ProductStoreLocationEntity(
                src.StoreLocationId, src.ProductId, src.Quantity));

        CreateMap<ProductStoreLocationQueryRow, ProductStoreLocationDetails>();
    }
}
