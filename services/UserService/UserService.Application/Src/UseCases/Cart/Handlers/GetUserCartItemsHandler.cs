using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Persistence;
using UserService.Application.UseCases.Cart.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Handlers;

public class GetUserCartItemsHandler : IRequestHandler<GetUserCartItemsQuery, Result<List<CartProductEntity>>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetUserCartItemsHandler> _logger;

    public GetUserCartItemsHandler(ILogger<GetUserCartItemsHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<CartProductEntity>>> Handle(GetUserCartItemsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handler");
        _logger.LogDebug("Getting cart items for user: {Userid}", request.UserId);
        string userId = request.UserId.ToString();
        List<CartProductEntity> result = await _context.CartProducts
            .Where(cp => cp.UserId == userId)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogDebug("Retrieved cart items: {@CartProducts}", result);
        return new Result<List<CartProductEntity>>(result);
    }
}