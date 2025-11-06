using System.Collections.Generic;

namespace MyApp.ApiService.Contracts.Orders;

public record CreateOrderRequest(IReadOnlyCollection<CreateOrderLineRequest> Lines);
