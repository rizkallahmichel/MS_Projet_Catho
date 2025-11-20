namespace MyApp.WebApp.Clients.Models;

public record PharmacySummary(
    Guid Id,
    string Name,
    bool IsActive,
    int CategoriesCount,
    int ProductsCount);
