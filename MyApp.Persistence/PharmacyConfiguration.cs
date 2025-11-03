using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Pharmacies;

namespace MyApp.Persistence;

public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
{
    public void Configure(EntityTypeBuilder<Pharmacy> builder)
    {
        builder.ToTable("Pharmacies");
        builder.HasKey(pharmacy => pharmacy.Id);

        builder.Property(pharmacy => pharmacy.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pharmacy => pharmacy.Description)
            .HasMaxLength(500);

        builder.Property(pharmacy => pharmacy.Address)
            .HasMaxLength(400);

        builder.Property(pharmacy => pharmacy.IsActive)
            .IsRequired();

        builder.Property(pharmacy => pharmacy.CreatedAt)
            .IsRequired();

        builder.HasMany(pharmacy => pharmacy.Categories)
            .WithOne()
            .HasForeignKey(category => category.PharmacyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pharmacy => pharmacy.Products)
            .WithOne()
            .HasForeignKey(product => product.PharmacyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(pharmacy => pharmacy.Categories)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(pharmacy => pharmacy.Products)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
