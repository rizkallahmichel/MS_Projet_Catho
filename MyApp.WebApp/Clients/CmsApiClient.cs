using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MyApp.WebApp.Clients.Models;
using MyApp.WebApp.Clients.Models.Requests;
using MyApp.WebApp.Clients.Models.Shared;

namespace MyApp.WebApp.Clients;

public class CmsApiClient : ICmsApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public CmsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<PharmacySummary>> GetPharmaciesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<PharmacySummary>>(
            "api/pharmacies",
            SerializerOptions,
            cancellationToken);

        return result ?? Array.Empty<PharmacySummary>();
    }

    public async Task<PharmacyDetailsModel?> GetPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/pharmacies/{pharmacyId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PharmacyDetailsModel>(SerializerOptions, cancellationToken);
    }

    public async Task<Guid> CreatePharmacyAsync(CreatePharmacyRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pharmacies", request, SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(SerializerOptions, cancellationToken);
        return payload?.Id ?? Guid.Empty;
    }

    public async Task UpdatePharmacyAsync(Guid pharmacyId, UpdatePharmacyRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/pharmacies/{pharmacyId}", request, SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/pharmacies/{pharmacyId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<CategoryModel>>(
            $"api/pharmacies/{pharmacyId}/categories",
            SerializerOptions,
            cancellationToken);

        return result ?? Array.Empty<CategoryModel>();
    }

    public async Task<Guid> CreateCategoryAsync(Guid pharmacyId, CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/pharmacies/{pharmacyId}/categories",
            request,
            SerializerOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(SerializerOptions, cancellationToken);
        return payload?.Id ?? Guid.Empty;
    }

    public async Task UpdateCategoryAsync(Guid pharmacyId, Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/pharmacies/{pharmacyId}/categories/{categoryId}",
            request,
            SerializerOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(Guid pharmacyId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/pharmacies/{pharmacyId}/categories/{categoryId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<ProductModel>> GetProductsAsync(Guid pharmacyId, Guid? categoryId = null, CancellationToken cancellationToken = default)
    {
        var uri = categoryId.HasValue
            ? $"api/pharmacies/{pharmacyId}/products?categoryId={categoryId}"
            : $"api/pharmacies/{pharmacyId}/products";

        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<ProductModel>>(uri, SerializerOptions, cancellationToken);

        return result ?? Array.Empty<ProductModel>();
    }

    public async Task<Guid> CreateProductAsync(Guid pharmacyId, CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/pharmacies/{pharmacyId}/products",
            request,
            SerializerOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(SerializerOptions, cancellationToken);
        return payload?.Id ?? Guid.Empty;
    }

    public async Task UpdateProductAsync(Guid pharmacyId, Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/pharmacies/{pharmacyId}/products/{productId}",
            request,
            SerializerOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProductAsync(Guid pharmacyId, Guid productId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/pharmacies/{pharmacyId}/products/{productId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<OrderSummaryModel>> GetOrdersAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<OrderSummaryModel>>(
            $"api/pharmacies/{pharmacyId}/orders",
            SerializerOptions,
            cancellationToken);

        return result ?? Array.Empty<OrderSummaryModel>();
    }

    public async Task<OrderCreatedResult> CreateOrderAsync(Guid pharmacyId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/pharmacies/{pharmacyId}/orders",
            request,
            SerializerOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OrderCreatedResponse>(SerializerOptions, cancellationToken);
        if (payload is null)
        {
            return new OrderCreatedResult(Guid.Empty, string.Empty);
        }

        return new OrderCreatedResult(payload.Id, payload.OrderNumber);
    }

    public async Task<IReadOnlyList<PaymentModel>> GetPaymentsAsync(Guid pharmacyId, Guid? orderId = null, CancellationToken cancellationToken = default)
    {
        var uri = orderId.HasValue
            ? $"api/pharmacies/{pharmacyId}/payments?orderId={orderId}"
            : $"api/pharmacies/{pharmacyId}/payments";

        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<PaymentModel>>(uri, SerializerOptions, cancellationToken);
        return result ?? Array.Empty<PaymentModel>();
    }

    public async Task<PharmacyDetailsModel?> GetManagedPharmacyAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/pharmacies/mine", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PharmacyDetailsModel>(SerializerOptions, cancellationToken);
    }

    private sealed record IdResponse(Guid Id);
    private sealed record OrderCreatedResponse(Guid Id, string OrderNumber);
}
