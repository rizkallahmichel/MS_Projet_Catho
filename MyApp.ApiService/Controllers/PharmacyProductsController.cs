using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Products;
using MyApp.Application.Common.Models;
using MyApp.Application.Products.Commands.CreateProduct;
using MyApp.Application.Products.Commands.DeleteProduct;
using MyApp.Application.Products.Commands.UpdateProduct;
using MyApp.Application.Products.Queries.GetProductsByPharmacy;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/pharmacies/{pharmacyId:guid}/products")]
[Authorize]
public class PharmacyProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(
        Guid pharmacyId,
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetProductsByPharmacyQuery(pharmacyId, categoryId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminOrPharmacyRole")]
    public async Task<ActionResult<object>> CreateProduct(
        Guid pharmacyId,
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var productId = await _mediator.Send(
            new CreateProductCommand(
                pharmacyId,
                request.CategoryId,
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity),
            cancellationToken);

        return CreatedAtAction(nameof(GetProducts), new { pharmacyId }, new { id = productId });
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Policy = "RequireAdminOrPharmacyRole")]
    public async Task<IActionResult> UpdateProduct(
        Guid pharmacyId,
        Guid productId,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateProductCommand(
                pharmacyId,
                productId,
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity,
                request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    [Authorize(Policy = "RequireAdminOrPharmacyRole")]
    public async Task<IActionResult> DeleteProduct(Guid pharmacyId, Guid productId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand(pharmacyId, productId), cancellationToken);
        return NoContent();
    }
}
