using MyApp.WebApp.Clients.Models.Shared;

namespace MyApp.WebApp.Clients.Models;

public record PharmacyDetailsModel(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<CategoryModel> Categories,
    IReadOnlyCollection<ProductModel> Products);
