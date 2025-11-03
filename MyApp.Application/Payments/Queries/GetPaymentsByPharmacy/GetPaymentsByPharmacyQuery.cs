using MediatR;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Payments.Queries.GetPaymentsByPharmacy;

public record GetPaymentsByPharmacyQuery(Guid PharmacyId, Guid? OrderId) : IRequest<IReadOnlyList<PaymentDto>>;
