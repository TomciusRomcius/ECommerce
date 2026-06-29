using MediatR;

namespace UserService.Application.UseCases.ShippingAddress.Commands;

public record RemoveShippingAddressCommand(Guid UserId) : IRequest;