namespace MyApp.Domain.Orders;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}
