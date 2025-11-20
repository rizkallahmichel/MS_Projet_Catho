using System.Linq;
using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Orders;
using MyApp.Domain.Payments;

namespace MyApp.Application.Payments.Commands.EnsurePayOnDeliveryPayment;

public class EnsurePayOnDeliveryPaymentCommandHandler
    : IRequestHandler<EnsurePayOnDeliveryPaymentCommand, PaymentDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnsurePayOnDeliveryPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentDto> Handle(EnsurePayOnDeliveryPaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.OrderId == Guid.Empty)
        {
            throw new InvalidOperationException("A valid order identifier is required.");
        }

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null || order.PharmacyId != request.PharmacyId)
        {
            throw new InvalidOperationException("Order could not be found for this pharmacy.");
        }

        var existing = (await _paymentRepository.GetByOrderAsync(order.Id, cancellationToken)).FirstOrDefault();
        if (existing is not null)
        {
            return Map(existing);
        }

        var payment = Payment.Create(
            order.Id,
            order.TotalAmount,
            DateTimeOffset.UtcNow,
            "Pending",
            "PayOnDelivery");

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(payment);
    }

    private static PaymentDto Map(Payment payment) =>
        new(payment.Id, payment.OrderId, payment.Amount, payment.PaidAt, payment.Status, payment.Method);
}
