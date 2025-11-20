using MyApp.Domain.Common;

namespace MyApp.Domain.Orders;

public class OrderLine : Entity
{
    private OrderLine()
    {
        // EF Core
    }

    private OrderLine(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;

    public static OrderLine Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);

        return new OrderLine(Guid.NewGuid(), orderId, productId, quantity, unitPrice);
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Quantity = quantity;
    }
}
