using ECommerceBackend.Utils.Pagination;
using MediatR;
using ProductService.Domain.Entities;

namespace ProductService.Application.UseCases.Product.Queries;

public record GetProductsQuery(string searchText, int PageNumber, int PageSize) : IRequest<Page<ProductEntity>>;
