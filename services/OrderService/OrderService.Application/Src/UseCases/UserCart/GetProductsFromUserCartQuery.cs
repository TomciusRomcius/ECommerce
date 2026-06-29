using MediatR;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.UserCart;

public record GetProductsFromUserCartQuery(Guid UserId) : IRequest<Result<List<CartProductMinimalModel>>>;
