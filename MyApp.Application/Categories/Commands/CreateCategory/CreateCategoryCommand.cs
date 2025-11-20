using MediatR;

namespace MyApp.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(Guid PharmacyId, string Name, string? Description) : IRequest<Guid>;
