using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Orders;

namespace MyApp.Persistence;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");
        builder.HasKey(line => line.Id);

        builder.Property(line => line.OrderId)
            .IsRequired();

        builder.Property(line => line.ProductId)
            .IsRequired();

        builder.Property(line => line.Quantity)
            .IsRequired();

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
