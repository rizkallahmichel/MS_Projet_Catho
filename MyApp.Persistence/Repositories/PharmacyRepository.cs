using System;
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

    public async Task<Pharmacy?> GetByManagerUserIdAsync(string managerUserId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(managerUserId);

        return await _dbContext.Pharmacies
            .Include(pharmacy => pharmacy.Categories)
            .Include(pharmacy => pharmacy.Products)
            .FirstOrDefaultAsync(pharmacy => pharmacy.ManagerUserId == managerUserId, cancellationToken);
    }

    public async Task<Pharmacy?> GetByManagerUsernameAsync(string managerUsername, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(managerUsername);

        return await _dbContext.Pharmacies
            .Include(pharmacy => pharmacy.Categories)
            .Include(pharmacy => pharmacy.Products)
            .FirstOrDefaultAsync(pharmacy => pharmacy.ManagerUsername == managerUsername, cancellationToken);
    }

    public async Task<IReadOnlyList<Pharmacy>> SearchByProductNameAsync(string productName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);

        var sanitizedValue = EscapeLikePattern(productName.Trim());
        var pattern = $"%{sanitizedValue}%";

        return await _dbContext.Pharmacies
            .Include(pharmacy => pharmacy.Categories)
            .Include(pharmacy => pharmacy.Products)
            .Where(pharmacy => pharmacy.Products.Any(product =>
                product.IsActive && EF.Functions.Like(product.Name, pattern)))
            .AsNoTracking()
            .OrderBy(pharmacy => pharmacy.Name)
            .ToListAsync(cancellationToken);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("[", "[[]", StringComparison.Ordinal)
            .Replace("%", "[%]", StringComparison.Ordinal)
            .Replace("_", "[_]", StringComparison.Ordinal);
    }
}
