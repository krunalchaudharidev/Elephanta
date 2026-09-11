using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressLine1).HasColumnType("text");
        builder.Property(x => x.AddressLine2).HasColumnType("text");
        builder.Property(x => x.City).HasColumnType("text");
        builder.Property(x => x.State).HasColumnType("text");
        builder.Property(x => x.PostalCode).HasColumnType("text");
        builder.Property(x => x.Country).HasColumnType("text");
        builder.Property(x => x.IsPrimary).HasColumnType("boolean");

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
