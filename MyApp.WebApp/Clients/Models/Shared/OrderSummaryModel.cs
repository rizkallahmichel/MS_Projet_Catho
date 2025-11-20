namespace MyApp.WebApp.Clients.Models.Shared;

public record OrderSummaryModel(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderedAt,
    string Status,
    decimal TotalAmount);
