using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Orders;

namespace MyApp.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.PharmacyId)
            .IsRequired();

        builder.Property(order => order.OrderNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(order => order.OrderedAt)
            .IsRequired();

        builder.Property(order => order.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(order => order.Lines)
            .WithOne()
            .HasForeignKey(line => line.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(order => new { order.PharmacyId, order.OrderNumber })
            .IsUnique();
    }
}
