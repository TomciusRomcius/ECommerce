using MediatR;
using ProductService.Domain.Utils;

namespace ProductService.Application.UseCases.Manufacturer.Commands;

public record CreateManufacturerCommand(string Name) : IRequest<Result<int>>;