using MediatR;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacies;

public record GetPharmaciesQuery : IRequest<IReadOnlyList<PharmacySummaryDto>>;
