namespace MyApp.ApiService.Contracts.Products;

public record CreateProductRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
