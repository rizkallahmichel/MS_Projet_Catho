using MediatR;
using MyApp.Application.Common.Models;
using MyApp.Domain.Orders;

namespace MyApp.Application.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryQueryHandler : IRequestHandler<GetOrderHistoryQuery, IReadOnlyList<OrderSummaryDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHistoryQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByPharmacyAsync(request.PharmacyId, cancellationToken);

        return orders
            .OrderByDescending(order => order.OrderedAt)
            .Select(order => new OrderSummaryDto(
                order.Id,
                order.OrderNumber,
                order.OrderedAt,
                order.Status,
                order.TotalAmount))
            .ToList();
    }
}
