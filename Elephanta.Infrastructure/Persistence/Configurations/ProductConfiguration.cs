using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.SKU)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.ShortDescription).HasColumnType("text");

        builder.Property(x => x.Description).HasColumnType("text");

        builder.Property(x => x.Price).HasColumnType("numeric");

        builder.Property(x => x.CompareAtPrice).HasColumnType("numeric");

        builder.Property(x => x.StockQuantity).HasColumnType("integer");

        builder.Property(x => x.IsActive).HasColumnType("boolean");

        builder.Property(x => x.IsFeatured).HasColumnType("boolean");

        builder.HasIndex(x => x.CategoryId);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
