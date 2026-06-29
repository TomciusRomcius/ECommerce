using MediatR;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;

namespace ProductService.Application.UseCases.Product.Queries;

public record GetProductsByIdQuery(List<int> ProductIds) : IRequest<Result<List<ProductEntity>>>;