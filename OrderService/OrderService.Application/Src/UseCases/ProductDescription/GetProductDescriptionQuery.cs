using MediatR;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.ProductDescription;

public record GetProductDescriptionQuery(List<int> ProductIds) : IRequest<Result<List<ProductPriceModel>>>;
