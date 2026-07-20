using MediatR;
using ProductService.Domain.Models;

namespace ProductService.Application.UseCases.Product.Commands;

public record UpdateProductCommand(int ProductId, UpdateProductModel Updator) : IRequest;
