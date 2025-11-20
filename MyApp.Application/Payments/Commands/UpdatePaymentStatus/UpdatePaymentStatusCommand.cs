using MediatR;

namespace MyApp.Application.Payments.Commands.UpdatePaymentStatus;

public record UpdatePaymentStatusCommand(Guid PharmacyId, Guid PaymentId, string Status) : IRequest;
