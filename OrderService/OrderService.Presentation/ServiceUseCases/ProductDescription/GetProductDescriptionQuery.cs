using MediatR;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.ServiceUseCases.ProductDescription;

/// <returns>A sorted list that is sorted on ProductId and value being price</returns>
/// <param name="ProductIds"></param>
public record GetProductDescriptionQuery(List<int> ProductIds) : IRequest<Result<List<ProductPriceModel>>>;
