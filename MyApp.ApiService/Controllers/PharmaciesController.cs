using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Pharmacies;
using MyApp.Application.Pharmacies.Commands.CreatePharmacy;
using MyApp.Application.Pharmacies.Commands.DeletePharmacy;
using MyApp.Application.Pharmacies.Commands.UpdatePharmacy;
using MyApp.Application.Pharmacies.Queries.GetPharmacies;
using MyApp.Application.Pharmacies.Queries.GetPharmacyById;
using MyApp.Application.Pharmacies.Queries.GetPharmacyByManagerUserId;
using MyApp.Application.Pharmacies.Queries.SearchPharmaciesByProduct;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/pharmacies")]
[Authorize]
public class PharmaciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmaciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PharmacySummaryDto>>> GetPharmacies(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetPharmaciesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("search-by-product")]
    public async Task<ActionResult<IReadOnlyList<PharmacyProductMatchDto>>> SearchPharmaciesByProduct(
        [FromQuery(Name = "productName")] string? productName,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new SearchPharmaciesByProductQuery(productName ?? string.Empty),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{pharmacyId:guid}")]
    public async Task<ActionResult<PharmacyDetailsDto>> GetPharmacyById(Guid pharmacyId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetPharmacyByIdQuery(pharmacyId), cancellationToken);
        return Ok(response);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "pharmacy")]
    public async Task<ActionResult<PharmacyDetailsDto>> GetManagedPharmacy(CancellationToken cancellationToken)
    {
        var managerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var managerUsername = User.FindFirstValue("preferred_username") ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(managerUserId) && string.IsNullOrWhiteSpace(managerUsername))
        {
            return Forbid();
        }

        var response = await _mediator.Send(new GetPharmacyByManagerUserIdQuery(managerUserId, managerUsername), cancellationToken);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<object>> CreatePharmacy(
        [FromBody] CreatePharmacyRequest request,
        CancellationToken cancellationToken)
    {
        var pharmacyId = await _mediator.Send(
            new CreatePharmacyCommand(
                request.Name,
                request.Description,
                request.Address,
                request.ManagerUsername,
                request.ManagerEmail,
                request.ManagerPassword,
                request.ManagerFirstName,
                request.ManagerLastName),
            cancellationToken);

        return CreatedAtAction(nameof(GetPharmacyById), new { pharmacyId }, new { id = pharmacyId });
    }

    [HttpPut("{pharmacyId:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> UpdatePharmacy(
        Guid pharmacyId,
        [FromBody] UpdatePharmacyRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdatePharmacyCommand(pharmacyId, request.Name, request.Description, request.Address, request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{pharmacyId:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeletePharmacy(Guid pharmacyId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePharmacyCommand(pharmacyId), cancellationToken);
        return NoContent();
    }
}
