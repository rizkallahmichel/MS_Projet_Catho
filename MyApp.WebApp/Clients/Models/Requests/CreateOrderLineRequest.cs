using System;

namespace MyApp.WebApp.Clients.Models.Requests;

public record CreateOrderLineRequest(Guid ProductId, int Quantity);
