using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Commands.DeletePharmacy;

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePharmacyCommandHandler(IPharmacyRepository pharmacyRepository, IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null)
        {
            throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.ToString());
        }

        await _pharmacyRepository.DeleteAsync(pharmacy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
