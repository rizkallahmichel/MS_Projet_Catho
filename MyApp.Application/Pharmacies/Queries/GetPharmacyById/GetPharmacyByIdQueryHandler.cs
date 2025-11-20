using MediatR;
using MyApp.Application.Common.Exceptions;
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

        return PharmacyDetailsMapper.ToDetailsDto(pharmacy);
    }
}
