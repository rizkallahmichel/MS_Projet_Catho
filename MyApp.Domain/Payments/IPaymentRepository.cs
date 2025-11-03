namespace MyApp.Domain.Payments;

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
