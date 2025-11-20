using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Categories;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        IPharmacyRepository pharmacyRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null)
        {
            throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.ToString());
        }

        var category = pharmacy.AddCategory(request.Name, request.Description);
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
