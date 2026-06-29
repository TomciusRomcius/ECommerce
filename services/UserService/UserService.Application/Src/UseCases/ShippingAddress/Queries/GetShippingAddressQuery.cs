using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.ShippingAddress.Queries;

public record GetShippingAddressesQuery(Guid UserId) : IRequest<List<ShippingAddressEntity>>;