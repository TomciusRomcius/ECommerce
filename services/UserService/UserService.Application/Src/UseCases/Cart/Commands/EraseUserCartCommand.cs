using MediatR;

namespace UserService.Application.UseCases.Cart.Commands;

public record EraseUserCartCommand(Guid UserId) : IRequest;