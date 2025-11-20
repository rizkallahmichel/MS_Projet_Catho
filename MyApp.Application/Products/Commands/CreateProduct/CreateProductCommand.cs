using MediatR;

namespace MyApp.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    Guid PharmacyId,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity) : IRequest<Guid>;
