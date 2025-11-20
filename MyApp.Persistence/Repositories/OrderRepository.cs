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

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
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
