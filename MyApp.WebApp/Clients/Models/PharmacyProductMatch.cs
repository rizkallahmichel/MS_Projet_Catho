using MyApp.WebApp.Clients.Models.Shared;

namespace MyApp.WebApp.Clients.Models;

public record PharmacyProductMatch(
    Guid PharmacyId,
    string PharmacyName,
    string? Address,
    bool IsActive,
    IReadOnlyCollection<ProductModel> Products);
