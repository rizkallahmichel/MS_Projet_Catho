namespace MyApp.WebApp.Clients.Models.Requests;

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    bool IsActive);
