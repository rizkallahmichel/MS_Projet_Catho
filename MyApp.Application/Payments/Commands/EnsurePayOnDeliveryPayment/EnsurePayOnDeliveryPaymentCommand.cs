using MediatR;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Payments.Commands.EnsurePayOnDeliveryPayment;

public record EnsurePayOnDeliveryPaymentCommand(Guid PharmacyId, Guid OrderId) : IRequest<PaymentDto>;
