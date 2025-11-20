using System.Linq;
using MyApp.Domain.Common;

namespace MyApp.Domain.Orders;

public class Order : Entity
{
    private readonly List<OrderLine> _lines = [];

    private Order()
    {
        // EF Core
    }

    private Order(
        Guid id,
        Guid pharmacyId,
        string orderNumber,
        DateTimeOffset orderedAt,
        string status,
        decimal totalAmount)
    {
        Id = id;
        PharmacyId = pharmacyId;
        OrderNumber = orderNumber;
        OrderedAt = orderedAt;
        Status = status;
        TotalAmount = totalAmount;
    }

    public Guid PharmacyId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public DateTimeOffset OrderedAt { get; private set; }
    public string Status { get; private set; } = null!;
    public decimal TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderLine> Lines => _lines;

    public static Order Create(
        Guid pharmacyId,
        string orderNumber,
        DateTimeOffset orderedAt,
        string status,
        IEnumerable<(Guid ProductId, int Quantity, decimal UnitPrice)> lines)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(orderNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(status);

        if (lines is null)
        {
            throw new ArgumentNullException(nameof(lines));
        }

        var preparedLines = lines.ToList();
        if (preparedLines.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one line.");
        }

        var order = new Order(Guid.NewGuid(), pharmacyId, orderNumber.Trim(), orderedAt, status.Trim(), 0m);
        decimal totalAmount = 0m;

        foreach (var line in preparedLines)
        {
            var orderLine = OrderLine.Create(order.Id, line.ProductId, line.Quantity, line.UnitPrice);
            order._lines.Add(orderLine);
            totalAmount += orderLine.LineTotal;
        }

        if (totalAmount <= 0)
        {
            throw new InvalidOperationException("Order total must be greater than zero.");
        }

        order.TotalAmount = totalAmount;

        return order;
    }

    public void UpdateStatus(string status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        Status = status.Trim();
    }
}
