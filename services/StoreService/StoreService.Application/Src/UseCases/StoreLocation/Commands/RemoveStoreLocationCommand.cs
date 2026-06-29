using MediatR;

namespace StoreService.Application.UseCases.StoreLocation.Commands;

public record RemoveStoreLocationCommand(int StoreLocationId) : IRequest;