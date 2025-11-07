using System;

namespace MyApp.WebApp.Clients.Models;

public record OrderCreatedResult(Guid OrderId, string OrderNumber);
