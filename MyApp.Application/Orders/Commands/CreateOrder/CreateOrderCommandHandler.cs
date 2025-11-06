using MediatR;
using MyApp.Domain.Abstractions;
using MyApp.Domain.Orders;
using MyApp.Domain.Pharmacies;
using MyApp.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IPharmacyRepository pharmacyRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _pharmacyRepository = pharmacyRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Lines is null || request.Lines.Count == 0)
        {
            throw new InvalidOperationException("Order must include at least one product.");
        }

        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId, cancellationToken);
        if (pharmacy is null || !pharmacy.IsActive)
        {
            throw new InvalidOperationException("Pharmacy is not available for orders.");
        }

        var aggregatedLines = request.Lines
            .GroupBy(line => line.ProductId)
            .Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToList();

        if (aggregatedLines.Any(line => line.Quantity <= 0))
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var orderLinePayload = new List<(Guid ProductId, int Quantity, decimal UnitPrice)>();

        foreach (var line in aggregatedLines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product is null || product.PharmacyId != request.PharmacyId || !product.IsActive)
            {
                throw new InvalidOperationException("One or more products are not available.");
            }

            if (product.StockQuantity < line.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'.");
            }

            product.DecreaseStock(line.Quantity);
            orderLinePayload.Add((product.Id, line.Quantity, product.Price));
        }

        var orderNumber = GenerateOrderNumber();

        var order = Order.Create(
            request.PharmacyId,
            orderNumber,
            DateTimeOffset.UtcNow,
            "Pending",
            orderLinePayload);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id, order.OrderNumber);
    }

    private static string GenerateOrderNumber()
    {
        var randomSegment = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"ORD-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{randomSegment}";
    }
}
