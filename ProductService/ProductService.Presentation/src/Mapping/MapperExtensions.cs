using AutoMapper;
using ECommerceBackend.Utils.Pagination;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Presentation.Controllers.Product.Dtos;

namespace ProductService.Presentation.Mapping;

public static class MapperExtensions
{
    public static Page<TDest> MapPage<TSource, TDest>(this IMapper mapper, Page<TSource> page) =>
        new()
        {
            Data = mapper.Map<List<TDest>>(page.Data),
            TotalCount = page.TotalCount,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage,
            PageSize = page.PageSize,
        };

    public static ProductWithStoreDto MapWithStoreDetails(
        this IMapper mapper,
        ProductEntity product,
        IReadOnlyDictionary<int, ProductStoreDetails> storeDetailsByProductId) =>
        mapper.Map<ProductWithStoreDto>(
            product,
            opt => opt.Items[PresentationMappingProfile.StoreDetailsByProductIdKey] = storeDetailsByProductId);

    public static List<ProductWithStoreDto> MapWithStoreDetails(
        this IMapper mapper,
        IEnumerable<ProductEntity> products,
        IReadOnlyDictionary<int, ProductStoreDetails> storeDetailsByProductId) =>
        products.Select(product => mapper.MapWithStoreDetails(product, storeDetailsByProductId)).ToList();
}
