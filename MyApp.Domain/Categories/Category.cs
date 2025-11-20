using MyApp.Domain.Common;

namespace MyApp.Domain.Categories;

public class Category : Entity
{
    private Category()
    {
        // EF Core
    }

    private Category(Guid id, Guid pharmacyId, string name, string? description, bool isActive)
    {
        Id = id;
        PharmacyId = pharmacyId;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid PharmacyId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Category Create(Guid pharmacyId, string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category(Guid.NewGuid(), pharmacyId, name.Trim(), description?.Trim(), true);
    }

    public void UpdateDetails(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Description = description?.Trim();
        Touch();
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            Touch();
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            Touch();
        }
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
