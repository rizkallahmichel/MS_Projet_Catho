using Moq;
using MyApp.Application.Orders.Commands.CreateOrder;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Orders;
using MyApp.Domain.Pharmacies;
using MyApp.Domain.Products;

namespace MyApp.Application.Tests.Orders;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IPharmacyRepository> _pharmacyRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_ShouldDecreaseStockAndPersistOrder()
    {
        var pharmacyId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var pharmacy = Pharmacy.Create("Test Pharmacy", null, null);
        var product = Product.Create(pharmacyId, Guid.NewGuid(), "Painkiller", null, 12.5m, 10);

        _pharmacyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(pharmacyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pharmacy);

        _productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        Order? capturedOrder = null;
        _orderRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((order, _) => capturedOrder = order)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateOrderCommandHandler(
            _pharmacyRepositoryMock.Object,
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var command = new CreateOrderCommand(
            pharmacyId,
            new List<CreateOrderLine>
            {
                new(productId, 3)
            });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.OrderId);
        Assert.False(string.IsNullOrWhiteSpace(result.OrderNumber));
        Assert.Equal(7, product.StockQuantity);
        Assert.NotNull(capturedOrder);
        Assert.Equal(pharmacyId, capturedOrder!.PharmacyId);
        Assert.Equal(37.5m, capturedOrder.TotalAmount);
        Assert.Single(capturedOrder.Lines);
        _orderRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenInsufficientStock()
    {
        var pharmacyId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var pharmacy = Pharmacy.Create("Test Pharmacy", null, null);
        var product = Product.Create(pharmacyId, Guid.NewGuid(), "Painkiller", null, 12.5m, 1);

        _pharmacyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(pharmacyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pharmacy);

        _productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new CreateOrderCommandHandler(
            _pharmacyRepositoryMock.Object,
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var command = new CreateOrderCommand(
            pharmacyId,
            new List<CreateOrderLine>
            {
                new(productId, 2)
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal(1, product.StockQuantity);
        _orderRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
