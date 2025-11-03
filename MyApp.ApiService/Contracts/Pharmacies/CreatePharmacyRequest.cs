namespace MyApp.ApiService.Contracts.Pharmacies;

public record CreatePharmacyRequest(
    string Name,
    string? Description,
    string? Address);
