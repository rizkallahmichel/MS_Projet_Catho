using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Models;
using MyApp.Application.Orders.Queries.GetOrderHistory;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/pharmacies/{pharmacyId:guid}/orders")]
[Authorize]
public class PharmacyOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> GetOrderHistory(Guid pharmacyId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOrderHistoryQuery(pharmacyId), cancellationToken);
        return Ok(response);
    }
}
