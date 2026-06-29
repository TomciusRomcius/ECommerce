using MediatR;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

/// <summary>
///     Subtracts stock from all ProductStoreLocationEntities' stock.
///     Stock = Stock - ProductStoreLocation.Stock
/// </summary>
public record UpdateProductsStockCommand(List<ProductStoreLocationEntity> ProductStoreLocations) : IRequest;