using MediatR;

namespace MyApp.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid PharmacyId, Guid CategoryId) : IRequest;
