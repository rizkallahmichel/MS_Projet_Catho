using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Contracts.Pharmacies;
using MyApp.ApiService.Controllers;
using MyApp.Application.Common.Models;
using MyApp.Application.Pharmacies.Commands.CreatePharmacy;
using MyApp.Application.Pharmacies.Commands.DeletePharmacy;
using MyApp.Application.Pharmacies.Commands.UpdatePharmacy;
using MyApp.Application.Pharmacies.Queries.GetPharmacies;
using MyApp.Application.Pharmacies.Queries.GetPharmacyById;

namespace MyApp.ApiService.Tests.Controllers;

public class PharmaciesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    private PharmaciesController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task GetPharmacies_ReturnsOkWithSummaries()
    {
        var summaries = new List<PharmacySummaryDto>
        {
            new(Guid.NewGuid(), "Main", true, 3, 15),
            new(Guid.NewGuid(), "Downtown", false, 1, 4)
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<GetPharmaciesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        var controller = CreateController();

        var result = await controller.GetPharmacies(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(summaries, okResult.Value);
        _mediator.Verify(m => m.Send(It.IsAny<GetPharmaciesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPharmacyById_ReturnsOkWithDetails()
    {
        var pharmacyId = Guid.NewGuid();
        var details = new PharmacyDetailsDto(
            pharmacyId,
            "Main",
            "Desc",
            "Address",
            true,
            DateTimeOffset.UtcNow,
            null,
            new List<CategoryDto>(),
            new List<ProductDto>());

        _mediator
            .Setup(m => m.Send(It.Is<GetPharmacyByIdQuery>(q => q.PharmacyId == pharmacyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        var controller = CreateController();

        var result = await controller.GetPharmacyById(pharmacyId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(details, okResult.Value);
    }

    [Fact]
    public async Task CreatePharmacy_ReturnsCreatedAtAction()
    {
        var expectedId = Guid.NewGuid();
        var request = new CreatePharmacyRequest(
            "Pharmacy",
            "Desc",
            "Address",
            "manager",
            "manager@example.com",
            "Password123!",
            "John",
            "Doe");

        _mediator
            .Setup(m => m.Send(It.IsAny<CreatePharmacyCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var controller = CreateController();

        var result = await controller.CreatePharmacy(request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PharmaciesController.GetPharmacyById), createdResult.ActionName);
        var value = createdResult.Value;
        Assert.NotNull(value);
        var idProperty = value!.GetType().GetProperty("id");
        Assert.NotNull(idProperty);
        Assert.Equal(expectedId, (Guid)idProperty!.GetValue(value)!);

        _mediator.Verify(m => m.Send(
                It.Is<CreatePharmacyCommand>(command =>
                    command.Name == request.Name &&
                    command.Description == request.Description &&
                    command.Address == request.Address &&
                    command.ManagerUsername == request.ManagerUsername &&
                    command.ManagerEmail == request.ManagerEmail &&
                    command.ManagerPassword == request.ManagerPassword &&
                    command.ManagerFirstName == request.ManagerFirstName &&
                    command.ManagerLastName == request.ManagerLastName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePharmacy_ReturnsNoContent()
    {
        var pharmacyId = Guid.NewGuid();
        var request = new UpdatePharmacyRequest("Updated", "Desc", "Addr", true);

        _mediator
            .Setup(m => m.Send(It.IsAny<UpdatePharmacyCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.UpdatePharmacy(pharmacyId, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<UpdatePharmacyCommand>(command =>
                    command.PharmacyId == pharmacyId &&
                    command.Name == request.Name &&
                    command.Description == request.Description &&
                    command.Address == request.Address &&
                    command.IsActive == request.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeletePharmacy_ReturnsNoContent()
    {
        var pharmacyId = Guid.NewGuid();

        _mediator
            .Setup(m => m.Send(It.IsAny<DeletePharmacyCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.DeletePharmacy(pharmacyId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<DeletePharmacyCommand>(command => command.PharmacyId == pharmacyId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
