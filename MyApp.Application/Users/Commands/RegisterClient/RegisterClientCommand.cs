using MediatR;

namespace MyApp.Application.Users.Commands.RegisterClient;

public sealed record RegisterClientCommand(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName) : IRequest<RegisterClientResult>;

public sealed record RegisterClientResult(string UserId);
