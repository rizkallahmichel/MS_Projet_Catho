using MediatR;

namespace MyApp.Application.Pharmacies.Queries.SearchPharmaciesByProduct;

public sealed record SearchPharmaciesByProductQuery(string ProductName) : IRequest<IReadOnlyList<PharmacyProductMatchDto>>;
