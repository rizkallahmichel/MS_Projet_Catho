using System.Collections.Generic;

namespace MyApp.WebApp.Clients.Models.Requests;

public record CreateOrderRequest(IReadOnlyCollection<CreateOrderLineRequest> Lines);
