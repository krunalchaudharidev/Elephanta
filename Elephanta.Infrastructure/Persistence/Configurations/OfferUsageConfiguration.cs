using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class OfferUsageConfiguration : IEntityTypeConfiguration<OfferUsage>
{
    public void Configure(EntityTypeBuilder<OfferUsage> builder)
    {
        builder.ToTable("OfferUsages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");

        builder.Property(x => x.UsedAt).IsRequired();

        builder.HasOne(x => x.Offer)
            .WithMany(o => o.Usages)
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
