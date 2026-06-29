using MediatR;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.StoreLocation.Queries;

public record GetStoreLocationsQuery(int PageNumber) : IRequest<List<StoreLocationEntity>>;