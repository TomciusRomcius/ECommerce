using MediatR;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

public record RemoveProductFromStoreCommand(int StoreLocationId, int ProductId) : IRequest;