using MediatR;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.ServiceUseCases.UserCart;

public record GetProductsFromUserCartQuery(Guid UserId) : IRequest<Result<List<CartProductMinimalModel>>>;