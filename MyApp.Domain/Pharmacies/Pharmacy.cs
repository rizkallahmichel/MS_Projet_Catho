using MyApp.Domain.Common;
using MyApp.Domain.Products;
using MyApp.Domain.Categories;

namespace MyApp.Domain.Pharmacies;

public class Pharmacy : Entity
{
    private readonly List<Category> _categories = [];
    private readonly List<Product> _products = [];

    private Pharmacy()
    {
        // Required by EF Core
    }

    private Pharmacy(Guid id, string name, string? description, string? address, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        Address = address;
        IsActive = isActive;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public string? ManagerUserId { get; private set; }
    public string? ManagerUsername { get; private set; }
    public string? ManagerEmail { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public static Pharmacy Create(string name, string? description, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Pharmacy(Guid.NewGuid(), name.Trim(), description?.Trim(), address?.Trim(), true);
    }

    public void UpdateDetails(string name, string? description, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Description = description?.Trim();
        Address = address?.Trim();
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

    public Category AddCategory(string name, string? description)
    {
        var category = Category.Create(Id, name, description);
        _categories.Add(category);
        return category;
    }

    public Product AddProduct(Guid categoryId, string name, string? description, decimal price, int stockQuantity)
    {
        var product = Product.Create(Id, categoryId, name, description, price, stockQuantity);
        _products.Add(product);
        return product;
    }

    public void RemoveCategory(Guid categoryId)
    {
        _categories.RemoveAll(category => category.Id == categoryId);
    }

    public void RemoveProduct(Guid productId)
    {
        _products.RemoveAll(product => product.Id == productId);
    }

    public void AssignManager(string managerUserId, string managerUsername, string managerEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(managerUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(managerUsername);
        ArgumentException.ThrowIfNullOrWhiteSpace(managerEmail);

        ManagerUserId = managerUserId;
        ManagerUsername = managerUsername.Trim();
        ManagerEmail = managerEmail.Trim();
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
