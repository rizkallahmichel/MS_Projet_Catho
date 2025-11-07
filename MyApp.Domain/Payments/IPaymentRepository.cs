namespace MyApp.Domain.Payments;

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
