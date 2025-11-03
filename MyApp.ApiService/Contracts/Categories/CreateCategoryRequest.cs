namespace MyApp.ApiService.Contracts.Categories;

public record CreateCategoryRequest(
    string Name,
    string? Description);
