using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class OrderAddressConfiguration : IEntityTypeConfiguration<OrderAddress>
{
    public void Configure(EntityTypeBuilder<OrderAddress> builder)
    {
        builder.ToTable("OrderAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.RecipientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        builder.Property(x => x.AddressLine1).IsRequired().HasMaxLength(300);
        builder.Property(x => x.AddressLine2).HasMaxLength(300);
        builder.Property(x => x.City).IsRequired().HasMaxLength(100);
        builder.Property(x => x.State).HasMaxLength(100);
        builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => new { x.OrderId, x.AddressType }).IsUnique();

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
