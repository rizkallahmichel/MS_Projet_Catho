using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Payments;
using MyApp.Application.Common.Models;
using MyApp.Application.Payments.Commands.EnsurePayOnDeliveryPayment;
using MyApp.Application.Payments.Commands.UpdatePaymentStatus;
using MyApp.Application.Payments.Queries.GetPaymentsByPharmacy;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/pharmacies/{pharmacyId:guid}/payments")]
[Authorize]
public class PharmacyPaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyPaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetPayments(
        Guid pharmacyId,
        [FromQuery] Guid? orderId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetPaymentsByPharmacyQuery(pharmacyId, orderId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment(
        Guid pharmacyId,
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || request.OrderId == Guid.Empty)
        {
            return BadRequest("A valid order identifier is required.");
        }

        var response = await _mediator.Send(
            new EnsurePayOnDeliveryPaymentCommand(pharmacyId, request.OrderId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{paymentId:guid}/status")]
    [Authorize(Policy = "RequireAdminOrPharmacyRole")]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid pharmacyId,
        Guid paymentId,
        [FromBody] UpdatePaymentStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest("A payment status is required.");
        }

        await _mediator.Send(
            new UpdatePaymentStatusCommand(pharmacyId, paymentId, request.Status),
            cancellationToken);

        return NoContent();
    }
}
