namespace MyApp.Application.Common.Models;

public record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderedAt,
    string Status,
    decimal TotalAmount);
