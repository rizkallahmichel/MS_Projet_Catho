using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Payments;

namespace MyApp.Persistence;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.OrderId)
            .IsRequired();

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.PaidAt)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(payment => payment.Method)
            .IsRequired()
            .HasMaxLength(50);
    }
}
