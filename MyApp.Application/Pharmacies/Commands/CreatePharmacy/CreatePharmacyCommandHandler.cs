using MediatR;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Guid>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePharmacyCommandHandler(IPharmacyRepository pharmacyRepository, IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = Pharmacy.Create(request.Name, request.Description, request.Address);
        await _pharmacyRepository.AddAsync(pharmacy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return pharmacy.Id;
    }
}
