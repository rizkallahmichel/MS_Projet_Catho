using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Categories;

namespace MyApp.Application.Categories.Queries.GetCategoriesByPharmacy;

public class GetCategoriesByPharmacyQueryHandler : IRequestHandler<GetCategoriesByPharmacyQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesByPharmacyQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesByPharmacyQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetByPharmacyAsync(request.PharmacyId, cancellationToken);

        return categories
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive))
            .ToList();
    }
}
