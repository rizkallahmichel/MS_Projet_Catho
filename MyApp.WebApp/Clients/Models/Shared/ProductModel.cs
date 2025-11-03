namespace MyApp.WebApp.Clients.Models.Shared;

public record ProductModel(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive);
