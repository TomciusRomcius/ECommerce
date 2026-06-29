using MediatR;
using ProductService.Domain.Entities;

namespace ProductService.Application.UseCases.Product.Queries;

public record GetProductByNameQuery(string Name) : IRequest<ProductEntity?>;