using MediatR;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Categories.Queries.GetCategoriesByPharmacy;

public record GetCategoriesByPharmacyQuery(Guid PharmacyId) : IRequest<IReadOnlyList<CategoryDto>>;
