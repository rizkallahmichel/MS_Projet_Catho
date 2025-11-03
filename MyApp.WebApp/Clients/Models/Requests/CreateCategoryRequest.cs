namespace MyApp.WebApp.Clients.Models.Requests;

public record CreateCategoryRequest(
    string Name,
    string? Description);
