using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Products;

namespace MyApp.Application.Products.Queries.GetProductsByPharmacy;

public class GetProductsByPharmacyQueryHandler : IRequestHandler<GetProductsByPharmacyQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsByPharmacyQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsByPharmacyQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<MyApp.Domain.Products.Product> products;

        if (request.CategoryId.HasValue)
        {
            products = await _productRepository.GetByCategoryAsync(request.PharmacyId, request.CategoryId.Value, cancellationToken);
        }
        else
        {
            products = await _productRepository.GetByPharmacyAsync(request.PharmacyId, cancellationToken);
        }

        return products
            .Select(product => new ProductDto(
                product.Id,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.IsActive))
            .ToList();
    }
}
