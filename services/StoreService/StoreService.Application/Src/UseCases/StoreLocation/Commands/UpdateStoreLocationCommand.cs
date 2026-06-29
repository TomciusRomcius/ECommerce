using MediatR;
using StoreService.Domain.Models;

namespace StoreService.Application.UseCases.StoreLocation.Commands;

public record UpdateStoreLocationCommand(UpdateStoreLocationModel Updator) : IRequest;