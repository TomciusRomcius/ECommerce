using MediatR;
using UserService.Domain.Entities;
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Commands;

public record UpdateCartItemQuantityCommand(CartProductEntity CartProduct) : IRequest<ResultError?>;