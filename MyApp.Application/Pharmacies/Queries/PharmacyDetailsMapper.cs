using System.Linq;
using MyApp.Application.Common.Models;
using MyApp.Application.Pharmacies.Queries.GetPharmacyById;
using MyApp.Domain.Pharmacies;

namespace MyApp.Application.Pharmacies.Queries;

internal static class PharmacyDetailsMapper
{
    public static PharmacyDetailsDto ToDetailsDto(Pharmacy pharmacy)
    {
        var categories = pharmacy.Categories
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive))
            .ToList();

        var products = pharmacy.Products
            .Select(product => new ProductDto(
                product.Id,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.IsActive))
            .ToList();

        return new PharmacyDetailsDto(
            pharmacy.Id,
            pharmacy.Name,
            pharmacy.Description,
            pharmacy.Address,
            pharmacy.IsActive,
            pharmacy.CreatedAt,
            pharmacy.UpdatedAt,
            categories,
            products);
    }
}
