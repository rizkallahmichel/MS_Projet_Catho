namespace MyApp.WebApp.Clients.Models.Shared;

public record PaymentModel(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    DateTimeOffset PaidAt,
    string Status,
    string Method);
