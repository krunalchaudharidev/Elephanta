using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class OfferImageConfiguration : IEntityTypeConfiguration<OfferImage>
{
    public void Configure(EntityTypeBuilder<OfferImage> builder)
    {
        builder.ToTable("OfferImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MediaId)
            .IsRequired(false);

        builder.HasOne(x => x.Media)
            .WithMany()
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Offer)
            .WithMany(o => o.Images)
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
