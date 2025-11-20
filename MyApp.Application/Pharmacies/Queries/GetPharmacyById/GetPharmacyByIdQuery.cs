using MediatR;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacyById;

public record GetPharmacyByIdQuery(Guid PharmacyId) : IRequest<PharmacyDetailsDto>;
