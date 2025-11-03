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
        decimal totalAmount,
        IEnumerable<OrderLine> lines)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(orderNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        ArgumentOutOfRangeException.ThrowIfNegative(totalAmount);

        var order = new Order(Guid.NewGuid(), pharmacyId, orderNumber.Trim(), orderedAt, status.Trim(), totalAmount);
        foreach (var line in lines)
        {
            order._lines.Add(line);
        }

        return order;
    }

    public void UpdateStatus(string status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        Status = status.Trim();
    }
}
