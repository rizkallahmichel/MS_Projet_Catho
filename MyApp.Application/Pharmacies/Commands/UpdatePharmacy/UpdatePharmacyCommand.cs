using MediatR;

namespace MyApp.Application.Pharmacies.Commands.UpdatePharmacy;

public record UpdatePharmacyCommand(Guid PharmacyId, string Name, string? Description, string? Address, bool IsActive) : IRequest;
