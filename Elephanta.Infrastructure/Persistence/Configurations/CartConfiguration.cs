using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SessionId).HasColumnType("character varying");
        builder.Property(x => x.CouponCode).HasColumnType("character varying");

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("\"UserId\" IS NOT NULL");
        builder.HasIndex(x => x.SessionId)
            .IsUnique()
            .HasFilter("\"SessionId\" IS NOT NULL");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Offer)
            .WithMany()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
