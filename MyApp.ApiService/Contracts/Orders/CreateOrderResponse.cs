using System;

namespace MyApp.ApiService.Contracts.Orders;

public record CreateOrderResponse(Guid Id, string OrderNumber);
