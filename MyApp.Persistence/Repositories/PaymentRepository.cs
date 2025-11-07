using System;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Payments;

namespace MyApp.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly MyAppDbContext _dbContext;

    public PaymentRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Payment>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .Join(
                _dbContext.Orders,
                payment => payment.OrderId,
                order => order.Id,
                (payment, order) => new { payment, order })
            .Where(tuple => tuple.order.PharmacyId == pharmacyId)
            .Select(tuple => tuple.payment)
            .AsNoTracking()
            .OrderByDescending(payment => payment.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .Where(payment => payment.OrderId == orderId)
            .AsNoTracking()
            .OrderByDescending(payment => payment.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);
        await _dbContext.Payments.AddAsync(payment, cancellationToken);
    }
}
