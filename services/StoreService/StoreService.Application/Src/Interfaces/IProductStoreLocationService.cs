using StoreService.Domain.Entities;
using StoreService.Domain.Models;
using StoreService.Domain.Utils;

namespace StoreService.Application.Interfaces;

public interface IProductStoreLocationService
{
    public Task<ResultError?> AddProductToStore(ProductStoreLocationEntity entity);
    public Task RemoveProductFromStore(int storeLocationId, int productId);
    public Task UpdateProduct(ProductStoreLocationEntity entity);
    // TODO: define UpdateStock
    // public Task UpdateStock(List<CartProductEntity> cartProducts);
    public Task<List<DetailedProductModel>> GetProductsFromStoreAsync(int storeLocationId);
    public Task<List<ProductStoreLocationEntity>> GetProductsFromStoreAsync(List<(int, int)> storeLocationIdProductId);
    public Task<List<int>> GetProductIdsFromStoreAsync(int storeLocationId);
}