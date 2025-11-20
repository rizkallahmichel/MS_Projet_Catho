using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Commands.UpdatePharmacy;

public class UpdatePharmacyCommandHandler : IRequestHandler<UpdatePharmacyCommand>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePharmacyCommandHandler(IPharmacyRepository pharmacyRepository, IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null)
        {
            throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.ToString());
        }

        pharmacy.UpdateDetails(request.Name, request.Description, request.Address);

        if (request.IsActive)
        {
            pharmacy.Activate();
        }
        else
        {
            pharmacy.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
