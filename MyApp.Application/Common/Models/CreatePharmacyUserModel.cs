namespace MyApp.Application.Common.Models;

public sealed record CreatePharmacyUserModel(
    Guid PharmacyId,
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName);
