using MediatR;
using ProductService.Domain.Models;

namespace ProductService.Application.UseCases.Product.Commands;

public record UpdateProductCommand(UpdateProductModel Updator) : IRequest;
