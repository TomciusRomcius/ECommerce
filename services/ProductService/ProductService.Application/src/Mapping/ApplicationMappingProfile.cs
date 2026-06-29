using AutoMapper;
using ProductService.Application.Services;

namespace ProductService.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<StoreServiceProductStoreLocationDto, ProductStoreDetails>();
    }
}
