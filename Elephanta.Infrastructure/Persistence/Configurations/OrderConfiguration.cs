using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Currency)
            .HasMaxLength(3);

        builder.Property(x => x.Subtotal).HasColumnType("decimal(18,4)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.ShippingAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
