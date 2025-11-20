namespace MyApp.WebApp.Clients.Models.Requests;

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive);
