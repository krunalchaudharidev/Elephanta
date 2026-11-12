using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).IsRequired();

        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,4)");

        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .IsUnique()
            .HasFilter("\"CartId\" IS NOT NULL");

        builder.HasOne(x => x.Cart)
            .WithMany()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

    }
}
