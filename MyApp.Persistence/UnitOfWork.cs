using MyApp.Domain.Abstractions;

namespace MyApp.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyAppDbContext _dbContext;

    public UnitOfWork(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
