using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Categories;
using MyApp.Domain.Orders;
using MyApp.Domain.Payments;
using MyApp.Domain.Pharmacies;
using MyApp.Domain.Products;
using MyApp.Persistence.Repositories;

namespace MyApp.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services)
    {
        services.AddScoped<IPharmacyRepository, PharmacyRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
