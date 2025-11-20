using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Orders;
using MyApp.Application.Common.Models;
using MyApp.Application.Orders.Commands.CreateOrder;
using MyApp.Application.Orders.Queries.GetOrderHistory;
using System.Linq;

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

    [HttpPost]
    [Authorize(Policy = "RequireClientRole")]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(
        Guid pharmacyId,
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (request?.Lines is null || request.Lines.Count == 0)
        {
            return BadRequest("Order must include at least one line.");
        }

        var command = new CreateOrderCommand(
            pharmacyId,
            request.Lines
                .Select(line => new CreateOrderLine(line.ProductId, line.Quantity))
                .ToArray());

        var result = await _mediator.Send(command, cancellationToken);
        var response = new CreateOrderResponse(result.OrderId, result.OrderNumber);

        return CreatedAtAction(nameof(GetOrderHistory), new { pharmacyId }, response);
    }
}
