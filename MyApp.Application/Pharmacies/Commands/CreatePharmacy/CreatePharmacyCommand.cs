using MediatR;

namespace MyApp.Application.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand(
    string Name,
    string? Description,
    string? Address,
    string ManagerUsername,
    string ManagerEmail,
    string ManagerPassword,
    string? ManagerFirstName,
    string? ManagerLastName) : IRequest<Guid>;
