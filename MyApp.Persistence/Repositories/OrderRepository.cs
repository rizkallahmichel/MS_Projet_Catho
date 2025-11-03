using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Orders;

namespace MyApp.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MyAppDbContext _dbContext;

    public OrderRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Order>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Where(order => order.PharmacyId == pharmacyId)
            .AsNoTracking()
            .OrderByDescending(order => order.OrderedAt)
            .ToListAsync(cancellationToken);
    }
}
