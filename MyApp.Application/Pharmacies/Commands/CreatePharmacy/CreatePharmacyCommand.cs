using MediatR;

namespace MyApp.Application.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand(string Name, string? Description, string? Address) : IRequest<Guid>;
