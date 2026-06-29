using AutoMapper;
using ECommerceBackend.Utils.Pagination;

namespace BFF.Utils;

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
}