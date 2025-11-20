namespace MyApp.WebApp.Clients.Models.Requests;

public record CreatePharmacyRequest(
    string Name,
    string? Description,
    string? Address,
    string ManagerUsername,
    string ManagerEmail,
    string ManagerPassword,
    string? ManagerFirstName,
    string? ManagerLastName);
