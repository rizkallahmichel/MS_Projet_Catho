using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Products;

namespace MyApp.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MyAppDbContext _dbContext;

    public ProductRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(product => product.PharmacyId == pharmacyId)
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid pharmacyId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(product => product.PharmacyId == pharmacyId && product.CategoryId == categoryId)
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }
}
