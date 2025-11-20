using System;
using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Queries.SearchPharmaciesByProduct;

public sealed class SearchPharmaciesByProductQueryHandler
    : IRequestHandler<SearchPharmaciesByProductQuery, IReadOnlyList<PharmacyProductMatchDto>>
{
    private readonly IPharmacyRepository _pharmacyRepository;

    public SearchPharmaciesByProductQueryHandler(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }

    public async Task<IReadOnlyList<PharmacyProductMatchDto>> Handle(
        SearchPharmaciesByProductQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            return Array.Empty<PharmacyProductMatchDto>();
        }

        var searchTerm = request.ProductName.Trim();

        var pharmacies = await _pharmacyRepository.SearchByProductNameAsync(searchTerm, cancellationToken);

        var matches = pharmacies
            .Select(pharmacy =>
            {
                var matchedProducts = pharmacy.Products
                    .Where(product =>
                        product.IsActive &&
                        product.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(product => product.Name)
                    .Select(product => new ProductDto(
                        product.Id,
                        product.CategoryId,
                        product.Name,
                        product.Description,
                        product.Price,
                        product.StockQuantity,
                        product.IsActive))
                    .ToList();

                return new PharmacyProductMatchDto(
                    pharmacy.Id,
                    pharmacy.Name,
                    pharmacy.Address,
                    pharmacy.IsActive,
                    matchedProducts);
            })
            .Where(match => match.Products.Count > 0)
            .OrderBy(match => match.PharmacyName)
            .ToList();

        return matches;
    }
}
