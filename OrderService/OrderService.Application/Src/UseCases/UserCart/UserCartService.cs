using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.UseCases.ProductDescription;
using OrderService.Application.UseCases.StoreProducts;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.UserCart;

public class UserCartService : IUserCartService
{
    private readonly ILogger<UserCartService> _logger;
    private readonly IMediator _mediator;

    public UserCartService(ILogger<UserCartService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Result<IReadOnlyList<StoreProductModel>>> GetUserCartStoreProductsAsync(Guid userId)
    {
        _logger.LogTrace("Entered {}", nameof(GetUserCartStoreProductsAsync));
        _logger.LogDebug("Fetching user's {UserId} cart items.", userId);

        Result<List<CartProductMinimalModel>> userCartItemsResult =
            await _mediator.Send(new GetProductsFromUserCartQuery(userId));
        if (userCartItemsResult.Errors.Any())
        {
            return new Result<IReadOnlyList<StoreProductModel>>([userCartItemsResult.Errors.First()]);
        }

        List<CartProductMinimalModel> userCartItems = userCartItemsResult.GetValue();
        _logger.LogDebug("Minimal: {@minimal}", userCartItems);

        if (!userCartItems.Any())
        {
            _logger.LogDebug("User {UserId} has an empty cart.", userId);
            return new Result<IReadOnlyList<StoreProductModel>>(new List<StoreProductModel>());
        }

        List<int> productIds = userCartItems.Select(i => i.ProductId).Distinct().ToList();

        Result<List<ProductModel>> productDetailsResult =
            await _mediator.Send(new GetProductDescriptionQuery(productIds));
        if (productDetailsResult.Errors.Any())
        {
            _logger.LogError(
                "Failed to get product details of user cart items. User: {UserId} Errors: {@Errors}.",
                userId,
                productDetailsResult.Errors);

            return new Result<IReadOnlyList<StoreProductModel>>(productDetailsResult.Errors);
        }

        Result<List<StoreProductLocationModel>> storeProductsResult =
            await _mediator.Send(new GetStoreProductsQuery(productIds));
        if (storeProductsResult.Errors.Any())
        {
            _logger.LogError(
                "Failed to get store products for user cart items. User: {UserId} Errors: {@Errors}.",
                userId,
                storeProductsResult.Errors);

            return new Result<IReadOnlyList<StoreProductModel>>(storeProductsResult.Errors);
        }

        List<ProductModel> products = productDetailsResult.GetValue();
        List<StoreProductLocationModel> storeProducts = storeProductsResult.GetValue();
        _logger.LogDebug("Products: {@products}", products);
        _logger.LogDebug("Store products: {@storeProducts}", storeProducts);

        List<StoreProductModel> cartStoreProducts = [];
        foreach (CartProductMinimalModel cartItem in userCartItems)
        {
            ProductModel? product = products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
            StoreProductLocationModel? storeProduct = storeProducts.FirstOrDefault(
                sp => sp.ProductId == cartItem.ProductId && sp.StoreLocationId == cartItem.StoreLocationId);

            if (product is null)
            {
                _logger.LogError(
                    "Trying to fetch a product from user cart, but the product '{ProductId}' does not exist!",
                    cartItem.ProductId);

                return new Result<IReadOnlyList<StoreProductModel>>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Product does not exist!")
                ]);
            }

            if (storeProduct is null)
            {
                _logger.LogError(
                    "Trying to fetch a store product from user cart, but product '{ProductId}' at store '{StoreLocationId}' does not exist!",
                    cartItem.ProductId,
                    cartItem.StoreLocationId);

                return new Result<IReadOnlyList<StoreProductModel>>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Store product does not exist!")
                ]);
            }

            cartStoreProducts.Add(new StoreProductModel
            {
                Quantity = cartItem.Quantity,
                Stock = storeProduct.Stock,
                Product = product,
                StoreLocation = new StoreLocationModel
                {
                    StoreLocationId = storeProduct.StoreLocationId,
                    DisplayName = storeProduct.DisplayName,
                    Address = storeProduct.Address
                }
            });
        }

        return new Result<IReadOnlyList<StoreProductModel>>(cartStoreProducts);
    }
}
