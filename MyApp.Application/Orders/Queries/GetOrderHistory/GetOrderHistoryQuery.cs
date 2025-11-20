using MediatR;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Orders.Queries.GetOrderHistory;

public record GetOrderHistoryQuery(Guid PharmacyId) : IRequest<IReadOnlyList<OrderSummaryDto>>;
