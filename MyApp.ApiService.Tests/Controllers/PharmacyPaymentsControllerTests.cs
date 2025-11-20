using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Controllers;
using MyApp.Application.Common.Models;
using MyApp.Application.Payments.Queries.GetPaymentsByPharmacy;

namespace MyApp.ApiService.Tests.Controllers;

public class PharmacyPaymentsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Guid _pharmacyId = Guid.NewGuid();

    private PharmacyPaymentsController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task GetPayments_ReturnsOkWithData()
    {
        var payments = new List<PaymentDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), 15.5m, DateTimeOffset.UtcNow, "Completed", "Card"),
            new(Guid.NewGuid(), Guid.NewGuid(), 75m, DateTimeOffset.UtcNow.AddMinutes(-30), "Pending", "Cash")
        };

        _mediator
            .Setup(m => m.Send(It.Is<GetPaymentsByPharmacyQuery>(q => q.PharmacyId == _pharmacyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        var controller = CreateController();

        var result = await controller.GetPayments(_pharmacyId, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(payments, okResult.Value);
    }
}
