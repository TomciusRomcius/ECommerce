using MediatR;

namespace ProductService.Application.UseCases.Product.Commands;

public record DeleteProductCommand(int ProductId) : IRequest;
