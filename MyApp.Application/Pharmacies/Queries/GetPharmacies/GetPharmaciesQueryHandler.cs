using MediatR;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacies;

public class GetPharmaciesQueryHandler : IRequestHandler<GetPharmaciesQuery, IReadOnlyList<PharmacySummaryDto>>
{
    private readonly IPharmacyRepository _pharmacyRepository;

    public GetPharmaciesQueryHandler(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }

    public async Task<IReadOnlyList<PharmacySummaryDto>> Handle(GetPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var pharmacies = await _pharmacyRepository.GetAllAsync(cancellationToken);

        return pharmacies
            .Select(pharmacy => new PharmacySummaryDto(
                pharmacy.Id,
                pharmacy.Name,
                pharmacy.IsActive,
                pharmacy.Categories.Count,
                pharmacy.Products.Count))
            .ToList();
    }
}
