namespace MyApp.Application.Pharmacies.Queries.GetPharmacies;

public record PharmacySummaryDto(
    Guid Id,
    string Name,
    bool IsActive,
    int CategoriesCount,
    int ProductsCount);
