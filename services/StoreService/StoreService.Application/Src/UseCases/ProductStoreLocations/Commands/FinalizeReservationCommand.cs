using MediatR;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

public record FinalizeReservationCommand(Guid OrderId) : IRequest;
