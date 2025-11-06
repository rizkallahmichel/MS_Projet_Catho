namespace MyApp.WebApp.Clients.Models.Requests;

public sealed record RegisterClientRequest(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName);
