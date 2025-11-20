namespace MyApp.WebApp.Clients.Models.Shared;

public record CategoryModel(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);
