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

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(x => x.Offer)
            .WithMany() // Offer does not have collection property yet
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
