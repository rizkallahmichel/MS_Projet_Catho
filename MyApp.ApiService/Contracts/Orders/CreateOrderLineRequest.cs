using System;

namespace MyApp.ApiService.Contracts.Orders;

public record CreateOrderLineRequest(Guid ProductId, int Quantity);
