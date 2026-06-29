using MediatR;
using StoreService.Domain.Utils;

namespace StoreService.Application.UseCases.ProductStoreLocations.Commands;

public record ReserveProductItem(int StoreLocationId, int ProductId, int Stock);

public record ReserveProductsCommand(Guid OrderId, IReadOnlyList<ReserveProductItem> Products) : IRequest<ResultError?>;
