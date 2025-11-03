using MediatR;

namespace MyApp.Application.Pharmacies.Commands.DeletePharmacy;

public record DeletePharmacyCommand(Guid PharmacyId) : IRequest;
