using MyApp.WebApp.Clients.Models;
using MyApp.WebApp.Clients.Models.Requests;
using MyApp.WebApp.Clients.Models.Shared;

namespace MyApp.WebApp.Clients;

public interface ICmsApiClient
{
    Task<IReadOnlyList<PharmacySummary>> GetPharmaciesAsync(CancellationToken cancellationToken = default);
    Task<PharmacyDetailsModel?> GetPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<Guid> CreatePharmacyAsync(CreatePharmacyRequest request, CancellationToken cancellationToken = default);
    Task UpdatePharmacyAsync(Guid pharmacyId, UpdatePharmacyRequest request, CancellationToken cancellationToken = default);
    Task DeletePharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<Guid> CreateCategoryAsync(Guid pharmacyId, CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(Guid pharmacyId, Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid pharmacyId, Guid categoryId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductModel>> GetProductsAsync(Guid pharmacyId, Guid? categoryId = null, CancellationToken cancellationToken = default);
    Task<Guid> CreateProductAsync(Guid pharmacyId, CreateProductRequest request, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(Guid pharmacyId, Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid pharmacyId, Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderSummaryModel>> GetOrdersAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<OrderCreatedResult> CreateOrderAsync(Guid pharmacyId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentModel>> GetPaymentsAsync(Guid pharmacyId, Guid? orderId = null, CancellationToken cancellationToken = default);
    Task<PharmacyDetailsModel?> GetManagedPharmacyAsync(CancellationToken cancellationToken = default);
}
