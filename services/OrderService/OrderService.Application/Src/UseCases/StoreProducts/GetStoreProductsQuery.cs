using MediatR;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.StoreProducts;

public record GetStoreProductsQuery(List<int> ProductIds) : IRequest<Result<List<StoreProductLocationModel>>>;
