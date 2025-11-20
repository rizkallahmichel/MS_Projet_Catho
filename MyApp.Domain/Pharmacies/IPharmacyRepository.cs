namespace MyApp.Domain.Pharmacies;

public interface IPharmacyRepository
{
    Task<Pharmacy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pharmacy>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Pharmacy?> GetByManagerUserIdAsync(string managerUserId, CancellationToken cancellationToken = default);
    Task<Pharmacy?> GetByManagerUsernameAsync(string managerUsername, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pharmacy>> SearchByProductNameAsync(string productName, CancellationToken cancellationToken = default);
    Task AddAsync(Pharmacy pharmacy, CancellationToken cancellationToken = default);
    Task DeleteAsync(Pharmacy pharmacy, CancellationToken cancellationToken = default);
}
