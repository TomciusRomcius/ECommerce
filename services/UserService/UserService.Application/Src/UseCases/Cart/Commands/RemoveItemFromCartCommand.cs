using MediatR;
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Commands;

public record RemoveItemFromCartCommand(string UserId, int ProductId, int StoreLocationId) : IRequest<ResultError?>;
