using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Contracts.Categories;
using MyApp.ApiService.Controllers;
using MyApp.Application.Categories.Commands.CreateCategory;
using MyApp.Application.Categories.Commands.DeleteCategory;
using MyApp.Application.Categories.Commands.UpdateCategory;
using MyApp.Application.Categories.Queries.GetCategoriesByPharmacy;
using MyApp.Application.Common.Models;

namespace MyApp.ApiService.Tests.Controllers;

public class PharmacyCategoriesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Guid _pharmacyId = Guid.NewGuid();

    private PharmacyCategoriesController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task GetCategories_ReturnsOkWithData()
    {
        var categories = new List<CategoryDto>
        {
            new(Guid.NewGuid(), "Cold", "Desc", true),
            new(Guid.NewGuid(), "Supplements", null, true)
        };

        _mediator
            .Setup(m => m.Send(It.Is<GetCategoriesByPharmacyQuery>(q => q.PharmacyId == _pharmacyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var controller = CreateController();

        var result = await controller.GetCategories(_pharmacyId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(categories, okResult.Value);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtAction()
    {
        var request = new CreateCategoryRequest("Painkillers", "Fast relief");
        var categoryId = Guid.NewGuid();

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryId);

        var controller = CreateController();

        var result = await controller.CreateCategory(_pharmacyId, request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PharmacyCategoriesController.GetCategories), createdResult.ActionName);
        var value = createdResult.Value!;
        var idProperty = value.GetType().GetProperty("id");
        Assert.Equal(categoryId, (Guid)idProperty!.GetValue(value)!);

        _mediator.Verify(m => m.Send(
                It.Is<CreateCategoryCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.Name == request.Name &&
                    command.Description == request.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNoContent()
    {
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest("Updated", "Desc", true);

        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.UpdateCategory(_pharmacyId, categoryId, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<UpdateCategoryCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.CategoryId == categoryId &&
                    command.Name == request.Name &&
                    command.Description == request.Description &&
                    command.IsActive == request.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent()
    {
        var categoryId = Guid.NewGuid();

        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var controller = CreateController();

        var result = await controller.DeleteCategory(_pharmacyId, categoryId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _mediator.Verify(m => m.Send(
                It.Is<DeleteCategoryCommand>(command =>
                    command.PharmacyId == _pharmacyId &&
                    command.CategoryId == categoryId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
