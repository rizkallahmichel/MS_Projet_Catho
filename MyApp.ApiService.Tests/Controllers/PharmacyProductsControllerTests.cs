using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Contracts.Products;
using MyApp.ApiService.Controllers;
using MyApp.Application.Common.Models;
using MyApp.Application.Products.Commands.CreateProduct;
using MyApp.Application.Products.Commands.DeleteProduct;
using MyApp.Application.Products.Commands.UpdateProduct;
using MyApp.Application.Products.Queries.GetProductsByPharmacy;

namespace MyApp.ApiService.Tests.Controllers;

public class PharmacyProductsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Guid _pharmacyId = Guid.NewGuid();

    private PharmacyProductsController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task GetProducts_ReturnsOkWithData()
    {
        var products = new List<ProductDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Painkiller", "Relief", 10m, 5, true),
            new(Guid.NewGuid(), Guid.NewGuid(), "Vitamin C", null, 8m, 12, true)
        };

        _mediator
            .Setup(m => m.Send(It.Is<GetProductsByPharmacyQuery>(q => q.PharmacyId == _pharmacyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var controller = CreateController();

        var result = await controller.GetProducts(_pharmacyId, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(products, okResult.Value);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction()
    {
        var categoryId = Guid.NewGuid();
        var request = new CreateProductRequest(categoryId, "Painkiller", "Relief", 9.99m, 10);
        var productId = Guid.NewGuid();

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productId);

        var controller = CreateController();

        var result = await controller.CreateProduct(_pharmacyId, request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PharmacyProductsController.GetProducts), createdResult.ActionName);
        var value = createdResult.Value!;
        var idProperty = value.GetType().GetProperty("id");
        Assert.Equal(productId, (Guid)idProperty!.GetValue(value)!);

        _mediator.Verify(m => m.Send(
                It.Is<CreateProductCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.CategoryId == request.CategoryId &&
                    command.Name == request.Name &&
                    command.Description == request.Description &&
                    command.Price == request.Price &&
                    command.StockQuantity == request.StockQuantity),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContent()
    {
        var productId = Guid.NewGuid();
        var request = new UpdateProductRequest("Updated", "Desc", 11.5m, 7, true);

        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.UpdateProduct(_pharmacyId, productId, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<UpdateProductCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.ProductId == productId &&
                    command.Name == request.Name &&
                    command.Description == request.Description &&
                    command.Price == request.Price &&
                    command.StockQuantity == request.StockQuantity &&
                    command.IsActive == request.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent()
    {
        var productId = Guid.NewGuid();

        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.DeleteProduct(_pharmacyId, productId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<DeleteProductCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.ProductId == productId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
