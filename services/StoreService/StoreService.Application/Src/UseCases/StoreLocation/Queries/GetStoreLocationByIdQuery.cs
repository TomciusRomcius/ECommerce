using MediatR;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.StoreLocation.Queries;

public record GetStoreLocationByIdQuery(int StoreLocationId) : IRequest<StoreLocationEntity?>;
