using System.Collections.Generic;
using MediatR;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Orders;
using MyApp.Domain.Payments;

namespace MyApp.Application.Payments.Commands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand>
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Processed",
        "Done",
        "Paid",
        "Completed"
    };

    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentStatusCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw new InvalidOperationException("A payment status is required.");
        }

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            throw new InvalidOperationException("Payment could not be found.");
        }

        var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order is null || order.PharmacyId != request.PharmacyId)
        {
            throw new InvalidOperationException("This payment does not belong to the specified pharmacy.");
        }

        var normalizedStatus = NormalizeStatus(request.Status);
        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            throw new InvalidOperationException($"Unsupported payment status '{request.Status}'.");
        }

        var paymentNeedsUpdate = !string.Equals(payment.Status, normalizedStatus, StringComparison.OrdinalIgnoreCase);
        var orderNeedsUpdate = !string.Equals(order.Status, normalizedStatus, StringComparison.OrdinalIgnoreCase);

        if (!paymentNeedsUpdate && !orderNeedsUpdate)
        {
            return Unit.Value;
        }

        if (paymentNeedsUpdate)
        {
            payment.UpdateStatus(normalizedStatus);
        }

        if (orderNeedsUpdate)
        {
            order.UpdateStatus(normalizedStatus);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static string NormalizeStatus(string status)
    {
        var trimmed = status.Trim();
        if (trimmed.Length == 0)
        {
            return trimmed;
        }

        var lower = trimmed.ToLowerInvariant();
        return string.Concat(char.ToUpperInvariant(lower[0]), lower[1..]);
    }
}
