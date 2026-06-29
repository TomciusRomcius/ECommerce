using ECommerceBackend.Utils.Pagination;
using MediatR;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.AvailableProducts.Queries;

public record GetProductsFromStoreQuery(int StoreLocationId, int PageNumber, int PageSize)
    : IRequest<Page<ProductStoreLocationEntity>>;
