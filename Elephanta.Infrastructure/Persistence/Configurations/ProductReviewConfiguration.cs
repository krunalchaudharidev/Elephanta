using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rating).IsRequired().HasColumnType("integer");

        builder.Property(x => x.Comment).HasColumnType("text");

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Note: ProductReview has a UserId but no navigation property to User in the domain model.
        // Keep UserId as a nullable column and indexed; no FK navigation configured.
    }
}
