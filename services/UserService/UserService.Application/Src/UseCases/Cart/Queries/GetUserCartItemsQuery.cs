using MediatR;
using UserService.Domain.Entities;
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Queries;

public record GetUserCartItemsQuery(Guid UserId) : IRequest<Result<List<CartProductEntity>>>;