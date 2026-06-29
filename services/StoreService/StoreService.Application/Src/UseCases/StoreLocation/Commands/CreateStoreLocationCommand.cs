using MediatR;
using StoreService.Domain.Entities;
using StoreService.Domain.Models;

namespace StoreService.Application.UseCases.StoreLocation.Commands;

public record CreateStoreLocationCommand(CreateStoreLocationModel StoreLocation) : IRequest<StoreLocationEntity?>;