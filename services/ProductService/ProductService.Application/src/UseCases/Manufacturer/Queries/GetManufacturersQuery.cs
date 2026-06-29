using MediatR;
using ProductService.Domain.Entities;

namespace ProductService.Application.UseCases.Manufacturer.Queries;

public record GetManufacturersQuery(string SearchText) : IRequest<List<ManufacturerEntity>>;