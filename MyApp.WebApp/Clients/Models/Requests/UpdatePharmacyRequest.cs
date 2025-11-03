namespace MyApp.WebApp.Clients.Models.Requests;

public record UpdatePharmacyRequest(
    string Name,
    string? Description,
    string? Address,
    bool IsActive);
