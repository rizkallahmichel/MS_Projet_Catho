using MediatR;
using MyApp.Application.Common.Interfaces;
using MyApp.Application.Common.Models;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Guid>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityProvisioningService _identityProvisioningService;

    public CreatePharmacyCommandHandler(
        IPharmacyRepository pharmacyRepository,
        IUnitOfWork unitOfWork,
        IIdentityProvisioningService identityProvisioningService)
    {
        _pharmacyRepository = pharmacyRepository;
        _unitOfWork = unitOfWork;
        _identityProvisioningService = identityProvisioningService;
    }

    public async Task<Guid> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = Pharmacy.Create(request.Name, request.Description, request.Address);

        var identityRequest = new CreatePharmacyUserModel(
            pharmacy.Id,
            request.ManagerUsername,
            request.ManagerEmail,
            request.ManagerPassword,
            request.ManagerFirstName,
            request.ManagerLastName);

        var managerUserId = await _identityProvisioningService.ProvisionPharmacyUserAsync(identityRequest, cancellationToken);

        pharmacy.AssignManager(managerUserId, request.ManagerUsername, request.ManagerEmail);

        await _pharmacyRepository.AddAsync(pharmacy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return pharmacy.Id;
    }
}
