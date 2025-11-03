namespace MyApp.ApiService.Contracts.Pharmacies;

public record UpdatePharmacyRequest(
    string Name,
    string? Description,
    string? Address,
    bool IsActive);
