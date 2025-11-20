namespace MyApp.ApiService.Contracts.Categories;

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    bool IsActive);
