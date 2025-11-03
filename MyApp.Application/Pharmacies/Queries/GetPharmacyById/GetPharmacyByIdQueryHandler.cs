using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Application.Common.Models;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacyById;

public class GetPharmacyByIdQueryHandler : IRequestHandler<GetPharmacyByIdQuery, PharmacyDetailsDto>
{
    private readonly IPharmacyRepository _pharmacyRepository;

    public GetPharmacyByIdQueryHandler(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }

    public async Task<PharmacyDetailsDto> Handle(GetPharmacyByIdQuery request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null)
        {
            throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.ToString());
        }

        var categories = pharmacy.Categories
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive))
            .ToList();

        var products = pharmacy.Products
            .Select(product => new ProductDto(
                product.Id,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.IsActive))
            .ToList();

        return new PharmacyDetailsDto(
            pharmacy.Id,
            pharmacy.Name,
            pharmacy.Description,
            pharmacy.Address,
            pharmacy.IsActive,
            pharmacy.CreatedAt,
            pharmacy.UpdatedAt,
            categories,
            products);
    }
}
