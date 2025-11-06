using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ApiService.Contracts.Identity;
using MyApp.Application.Users.Commands.RegisterClient;

namespace MyApp.ApiService.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> RegisterClient([FromBody] RegisterClientRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterClientCommand(
                request.Username,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName),
            cancellationToken);

        return Created(string.Empty, new { id = result.UserId });
    }
}
