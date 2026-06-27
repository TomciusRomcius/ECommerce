using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.UseCases.ProductDescription;
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

    public async Task<Result<IEnumerable<CartProductModel>>> GetUserCartProductModelsAsync(Guid userId)
    {
        _logger.LogTrace("Entered {}", nameof(GetUserCartProductModelsAsync));
        _logger.LogDebug("Fetching user's {UserId} cart items.", userId);

        Result<List<CartProductMinimalModel>> userCartItemsResult =
            await _mediator.Send(new GetProductsFromUserCartQuery(userId));
        if (userCartItemsResult.Errors.Any())
        {
            return new Result<IEnumerable<CartProductModel>>([userCartItemsResult.Errors.First()]);
        }

        List<CartProductMinimalModel> userCartItems = userCartItemsResult.GetValue();
        _logger.LogInformation("Minimal: {@minimal}", userCartItems);
        Result<List<ProductPriceModel>> productDetailsResult = await _mediator.Send(
            new GetProductDescriptionQuery(userCartItems.Select(i => i.ProductId).ToList()));

        if (productDetailsResult.Errors.Any())
        {
            _logger.LogError(
                "Failed to get product details of user cart items. User: {UserId} Errors: {@Errors}.",
                userId,
                productDetailsResult.Errors);

            return new Result<IEnumerable<CartProductModel>>(productDetailsResult.Errors);
        }

        List<ProductPriceModel> productPriceModels = productDetailsResult.GetValue();
        _logger.LogInformation("productPriceModels: {@productPriceModels}", productPriceModels);

        List<CartProductModel> cartProducts = [];
        foreach (CartProductMinimalModel cartPr in userCartItems)
        {
            ProductPriceModel? cartPricingModel = productPriceModels
                .FirstOrDefault(i => i.ProductId == cartPr.ProductId);

            if (cartPricingModel == null)
            {
                _logger.LogError(
                    "Trying to fetch a product from user cart, but the product '{ProductId}' does not exist!",
                    cartPr.ProductId);

                return new Result<IEnumerable<CartProductModel>>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Product does not exist!")
                ]);
            }

            cartProducts.Add(new CartProductModel
            {
                ProductId = cartPr.ProductId,
                StoreLocationId = cartPr.StoreLocationId,
                Quantity = cartPr.Quantity,
                Price = cartPricingModel.Price
            });
        }

        _logger.LogInformation("final: {@final}", cartProducts);

        if (!cartProducts.Any())
        {
            _logger.LogDebug("User {UserId} has an empty cart.", userId);
        }

        return new Result<IEnumerable<CartProductModel>>(cartProducts);
    }
}
