using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Contracts.Orders;
using MyApp.ApiService.Controllers;
using MyApp.Application.Common.Models;
using MyApp.Application.Orders.Commands.CreateOrder;
using MyApp.Application.Orders.Queries.GetOrderHistory;
using System.Linq;

namespace MyApp.ApiService.Tests.Controllers;

public class PharmacyOrdersControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Guid _pharmacyId = Guid.NewGuid();

    private PharmacyOrdersController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task GetOrderHistory_ReturnsOkWithData()
    {
        var orders = new List<OrderSummaryDto>
        {
            new(Guid.NewGuid(), "ORD-1", DateTimeOffset.UtcNow, "Pending", 25m),
            new(Guid.NewGuid(), "ORD-2", DateTimeOffset.UtcNow.AddMinutes(-10), "Completed", 42m)
        };

        _mediator
            .Setup(m => m.Send(It.Is<GetOrderHistoryQuery>(q => q.PharmacyId == _pharmacyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var controller = CreateController();

        var result = await controller.GetOrderHistory(_pharmacyId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(orders, okResult.Value);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedAtAction()
    {
        var orderId = Guid.NewGuid();
        var orderNumber = "ORD-20231115-0001";
        var request = new CreateOrderRequest(new[]
        {
            new CreateOrderLineRequest(Guid.NewGuid(), 2),
            new CreateOrderLineRequest(Guid.NewGuid(), 1)
        });

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateOrderResult(orderId, orderNumber));

        var controller = CreateController();

        var result = await controller.CreateOrder(_pharmacyId, request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PharmacyOrdersController.GetOrderHistory), createdResult.ActionName);
        var response = Assert.IsType<CreateOrderResponse>(createdResult.Value);
        Assert.Equal(orderId, response.Id);
        Assert.Equal(orderNumber, response.OrderNumber);

        _mediator.Verify(m => m.Send(
                It.Is<CreateOrderCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.Lines.Count == request.Lines.Count &&
                    command.Lines.All(line =>
                        request.Lines.Any(req => req.ProductId == line.ProductId && req.Quantity == line.Quantity))),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenNoLines()
    {
        var controller = CreateController();
        var result = await controller.CreateOrder(_pharmacyId, new CreateOrderRequest(Array.Empty<CreateOrderLineRequest>()), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Order must include at least one line.", badRequest.Value);
        _mediator.Verify(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
