namespace MyApp.Application.Common.Models;

public sealed record CreateClientUserModel(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName);
