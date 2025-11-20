using MediatR;
using MyApp.Application.Common.Exceptions;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Products;

namespace MyApp.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null || product.PharmacyId != request.PharmacyId)
        {
            throw new NotFoundException(nameof(Product), request.ProductId.ToString());
        }

        product.UpdateDetails(request.Name, request.Description, request.Price, request.StockQuantity);

        if (request.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
