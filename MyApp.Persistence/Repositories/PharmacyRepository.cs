using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Pharmacies;

namespace MyApp.Persistence.Repositories;

public class PharmacyRepository : IPharmacyRepository
{
    private readonly MyAppDbContext _dbContext;

    public PharmacyRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Pharmacy pharmacy, CancellationToken cancellationToken = default)
    {
        await _dbContext.Pharmacies.AddAsync(pharmacy, cancellationToken);
    }

    public Task DeleteAsync(Pharmacy pharmacy, CancellationToken cancellationToken = default)
    {
        _dbContext.Pharmacies.Remove(pharmacy);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Pharmacy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pharmacies
            .Include(pharmacy => pharmacy.Categories)
            .Include(pharmacy => pharmacy.Products)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Pharmacy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pharmacies
            .Include(pharmacy => pharmacy.Categories)
            .Include(pharmacy => pharmacy.Products)
            .FirstOrDefaultAsync(pharmacy => pharmacy.Id == id, cancellationToken);
    }
}
