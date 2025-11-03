namespace MyApp.WebApp.Clients.Models.Requests;

public record CreatePharmacyRequest(
    string Name,
    string? Description,
    string? Address);
