using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Payments;

namespace MyApp.Application.Payments.Queries.GetPaymentsByPharmacy;

public class GetPaymentsByPharmacyQueryHandler : IRequestHandler<GetPaymentsByPharmacyQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByPharmacyQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<IReadOnlyList<PaymentDto>> Handle(GetPaymentsByPharmacyQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<MyApp.Domain.Payments.Payment> payments;
        if (request.OrderId.HasValue)
        {
            payments = await _paymentRepository.GetByOrderAsync(request.OrderId.Value, cancellationToken);
        }
        else
        {
            payments = await _paymentRepository.GetByPharmacyAsync(request.PharmacyId, cancellationToken);
        }

        return payments
            .OrderByDescending(payment => payment.PaidAt)
            .Select(payment => new PaymentDto(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.PaidAt,
                payment.Status,
                payment.Method))
            .ToList();
    }
}
