using MediatR;

namespace MyApp.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid PharmacyId,
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive) : IRequest;
