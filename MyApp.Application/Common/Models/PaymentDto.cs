namespace MyApp.Application.Common.Models;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    DateTimeOffset PaidAt,
    string Status,
    string Method);
