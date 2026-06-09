using MediatR;
using OrderService.Presentation.ServiceUseCases.ProductDescription;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.ServiceUseCases.UserCart;

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

        Result<List<CartProductMinimalModel>> userCartItemsResult = await _mediator.Send(new GetProductsFromUserCartQuery(userId));
        if (userCartItemsResult.Errors.Any())
        {
            return new Result<IEnumerable<CartProductModel>>([userCartItemsResult.Errors.First()]);
        }
        
        // Get cart products
        List<CartProductMinimalModel> userCartItems = userCartItemsResult.GetValue();
        _logger.LogInformation("Minimal: {@minimal}", userCartItems);
        Result<List<ProductPriceModel>> productDetailsResult = await
            _mediator.Send(new GetProductDescriptionQuery(userCartItems.Select(i => i.ProductId).ToList()));

        if (productDetailsResult.Errors.Any())
        {
            _logger.LogError(
                "Failed to get product details of user cart items. User: {UserId} Errors: {@Errors}.",
                userId,
                productDetailsResult.Errors
            );

            return new Result<IEnumerable<CartProductModel>>(productDetailsResult.Errors);
        }

        List<ProductPriceModel> productPriceModels = productDetailsResult.GetValue();
        _logger.LogInformation("productPriceModels: {@productPriceModels}", productPriceModels);

        // Assign price values to cart products
        List<CartProductModel> cartProducts = [];
        foreach (CartProductMinimalModel cartPr in userCartItems)
        {
            ProductPriceModel? cartPricingModel = productPriceModels
                .FirstOrDefault(i => i.ProductId == cartPr.ProductId);

            if (cartPricingModel == null)
            {
                _logger.LogError(
                    "Trying to fetch a product from user cart, but the product '{ProductId}' does not exist!",
                    cartPr.ProductId
                );

                return new Result<IEnumerable<CartProductModel>>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Product does not exist!")
                ]);
            }

            var cartProduct = new CartProductModel
            {
                ProductId = cartPr.ProductId,
                StoreLocationId = cartPr.StoreLocationId,
                Quantity = cartPr.Quantity,
                Price = cartPricingModel.Price
            };

            cartProducts.Add(cartProduct);
        }
        _logger.LogInformation("final: {@final}", cartProducts);

        return new Result<IEnumerable<CartProductModel>>(cartProducts);
    }
}
