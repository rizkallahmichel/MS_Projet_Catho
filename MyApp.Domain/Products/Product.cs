using MyApp.Domain.Common;

namespace MyApp.Domain.Products;

public class Product : Entity
{
    private Product()
    {
        // EF Core
    }

    private Product(
        Guid id,
        Guid pharmacyId,
        Guid categoryId,
        string name,
        string? description,
        decimal price,
        int stockQuantity,
        bool isActive)
    {
        Id = id;
        PharmacyId = pharmacyId;
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = isActive;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid PharmacyId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Product Create(Guid pharmacyId, Guid categoryId, string name, string? description, decimal price, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        if (stockQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stockQuantity));
        }

        return new Product(
            Guid.NewGuid(),
            pharmacyId,
            categoryId,
            name.Trim(),
            description?.Trim(),
            price,
            stockQuantity,
            true);
    }

    public void UpdateDetails(string name, string? description, decimal price, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        if (stockQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stockQuantity));
        }

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        StockQuantity = stockQuantity;
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

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > StockQuantity)
        {
            throw new InvalidOperationException("Insufficient stock available.");
        }

        StockQuantity -= quantity;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
