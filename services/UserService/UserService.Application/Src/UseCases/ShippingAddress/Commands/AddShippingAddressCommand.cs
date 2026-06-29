using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.ShippingAddress.Commands;

public record AddShippingAddressCommand(ShippingAddressEntity Address) : IRequest;