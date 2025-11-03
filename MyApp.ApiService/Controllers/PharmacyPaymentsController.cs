using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Models;
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
}
