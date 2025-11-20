namespace MyApp.Application.Common.Models;

public record ProductDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive);
