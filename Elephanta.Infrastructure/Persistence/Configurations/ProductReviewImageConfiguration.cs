using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class ProductReviewImageConfiguration : IEntityTypeConfiguration<ProductReviewImage>
{
    public void Configure(EntityTypeBuilder<ProductReviewImage> builder)
    {
        builder.ToTable("ProductReviewImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasColumnType("integer");

        builder.HasIndex(x => x.ProductReviewId);
        builder.HasIndex(x => x.MediaId);

        builder.HasOne(x => x.ProductReview)
            .WithMany(review => review.Images)
            .HasForeignKey(x => x.ProductReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Media)
            .WithMany()
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
