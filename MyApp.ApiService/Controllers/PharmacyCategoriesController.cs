using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Categories;
using MyApp.Application.Categories.Commands.CreateCategory;
using MyApp.Application.Categories.Commands.DeleteCategory;
using MyApp.Application.Categories.Commands.UpdateCategory;
using MyApp.Application.Categories.Queries.GetCategoriesByPharmacy;
using MyApp.Application.Common.Models;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/pharmacies/{pharmacyId:guid}/categories")]
[Authorize]
public class PharmacyCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(Guid pharmacyId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetCategoriesByPharmacyQuery(pharmacyId), cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacy")]
    public async Task<ActionResult<object>> CreateCategory(
        Guid pharmacyId,
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var categoryId = await _mediator.Send(
            new CreateCategoryCommand(pharmacyId, request.Name, request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetCategories), new { pharmacyId }, new { id = categoryId });
    }

    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = "Admin,Pharmacy")]
    public async Task<IActionResult> UpdateCategory(
        Guid pharmacyId,
        Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateCategoryCommand(pharmacyId, categoryId, request.Name, request.Description, request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = "Admin,Pharmacy")]
    public async Task<IActionResult> DeleteCategory(Guid pharmacyId, Guid categoryId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCategoryCommand(pharmacyId, categoryId), cancellationToken);
        return NoContent();
    }
}
