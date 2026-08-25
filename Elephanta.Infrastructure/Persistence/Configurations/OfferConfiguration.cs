using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.DiscountType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MinimumOrderAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MaximumDiscountAmount).HasColumnType("decimal(18,4)");

        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
