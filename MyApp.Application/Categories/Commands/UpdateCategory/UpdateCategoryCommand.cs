using MediatR;

namespace MyApp.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid PharmacyId,
    Guid CategoryId,
    string Name,
    string? Description,
    bool IsActive) : IRequest;
