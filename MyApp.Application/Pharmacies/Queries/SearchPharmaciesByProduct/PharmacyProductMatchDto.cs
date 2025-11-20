using MyApp.Application.Common.Models;

namespace MyApp.Application.Pharmacies.Queries.SearchPharmaciesByProduct;

public record PharmacyProductMatchDto(
    Guid PharmacyId,
    string PharmacyName,
    string? Address,
    bool IsActive,
    IReadOnlyCollection<ProductDto> Products);
