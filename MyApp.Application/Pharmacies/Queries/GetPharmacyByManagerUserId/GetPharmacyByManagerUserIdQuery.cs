using MediatR;
using MyApp.Application.Pharmacies.Queries.GetPharmacyById;

namespace MyApp.Application.Pharmacies.Queries.GetPharmacyByManagerUserId;

public sealed record GetPharmacyByManagerUserIdQuery(string? ManagerUserId, string? ManagerUsername) : IRequest<PharmacyDetailsDto?>;
