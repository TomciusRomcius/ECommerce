using MediatR;
using StoreService.Domain.Entities;
using StoreService.Domain.Utils;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

public record AddProductToStoreCommand(ProductStoreLocationEntity ProductStoreLocation) : IRequest<ResultError?>;