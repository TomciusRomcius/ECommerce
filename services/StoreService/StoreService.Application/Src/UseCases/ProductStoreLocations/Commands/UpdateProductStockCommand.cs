using MediatR;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

public record UpdateProductStockCommand(ProductStoreLocationEntity ProductStoreLocation) : IRequest;