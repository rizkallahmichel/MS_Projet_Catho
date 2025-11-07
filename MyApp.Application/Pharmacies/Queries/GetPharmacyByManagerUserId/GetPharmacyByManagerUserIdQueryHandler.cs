using MediatR;
using MyApp.Application.Pharmacies.Queries.GetPharmacyById;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacyByManagerUserId;

public sealed class GetPharmacyByManagerUserIdQueryHandler : IRequestHandler<GetPharmacyByManagerUserIdQuery, PharmacyDetailsDto?>
{
    private readonly IPharmacyRepository _pharmacyRepository;

    public GetPharmacyByManagerUserIdQueryHandler(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }

    public async Task<PharmacyDetailsDto?> Handle(GetPharmacyByManagerUserIdQuery request, CancellationToken cancellationToken)
    {
        Pharmacy? pharmacy = null;

        if (!string.IsNullOrWhiteSpace(request.ManagerUserId))
        {
            pharmacy = await _pharmacyRepository.GetByManagerUserIdAsync(request.ManagerUserId!, cancellationToken);
        }

        if (pharmacy is null && !string.IsNullOrWhiteSpace(request.ManagerUsername))
        {
            pharmacy = await _pharmacyRepository.GetByManagerUsernameAsync(request.ManagerUsername!, cancellationToken);
        }

        return pharmacy is null ? null : PharmacyDetailsMapper.ToDetailsDto(pharmacy);
    }
}
