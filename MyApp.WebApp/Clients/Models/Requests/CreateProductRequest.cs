namespace MyApp.WebApp.Clients.Models.Requests;

public record CreateProductRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
