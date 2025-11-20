namespace MyApp.Application.Common.Models;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);
