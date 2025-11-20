using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Categories;
using MyApp.Domain.Pharmacies;
using MyApp.Domain.Products;

namespace MyApp.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IPharmacyRepository pharmacyRepository,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null)
        {
            throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.ToString());
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || category.PharmacyId != request.PharmacyId)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId.ToString());
        }

        var product = pharmacy.AddProduct(
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
