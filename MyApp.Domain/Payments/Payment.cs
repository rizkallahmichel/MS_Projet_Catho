using MyApp.Domain.Common;

namespace MyApp.Domain.Payments;

public class Payment : Entity
{
    private Payment()
    {
        // EF Core
    }

    private Payment(
        Guid id,
        Guid orderId,
        decimal amount,
        DateTimeOffset paidAt,
        string status,
        string method)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        PaidAt = paidAt;
        Status = status;
        Method = method;
    }

    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset PaidAt { get; private set; }
    public string Status { get; private set; } = null!;
    public string Method { get; private set; } = null!;

    public static Payment Create(Guid orderId, decimal amount, DateTimeOffset paidAt, string status, string method)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        ArgumentException.ThrowIfNullOrWhiteSpace(method);

        return new Payment(Guid.NewGuid(), orderId, amount, paidAt, status.Trim(), method.Trim());
    }

    public void UpdateStatus(string status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        Status = status.Trim();
    }
}
