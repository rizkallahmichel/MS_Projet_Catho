using MyApp.Application.Common.Models;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacyById;

public record PharmacyDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<CategoryDto> Categories,
    IReadOnlyCollection<ProductDto> Products);
