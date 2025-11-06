using MediatR;

namespace MyApp.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid PharmacyId, IReadOnlyCollection<CreateOrderLine> Lines)
    : IRequest<CreateOrderResult>;

public record CreateOrderLine(Guid ProductId, int Quantity);

public record CreateOrderResult(Guid OrderId, string OrderNumber);
