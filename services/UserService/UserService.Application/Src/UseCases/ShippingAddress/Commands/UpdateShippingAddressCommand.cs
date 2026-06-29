using MediatR;
using UserService.Domain.Models;

namespace UserService.Application.UseCases.ShippingAddress.Commands;

public record UpdateShippingAddressCommand(UpdateShippingAddressModel UpdateAddress) : IRequest;