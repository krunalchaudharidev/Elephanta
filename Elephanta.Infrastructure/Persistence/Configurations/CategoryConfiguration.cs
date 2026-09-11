using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Description)
            .HasColumnType("text");

        builder.Property(x => x.DisplayOrder)
            .HasColumnType("integer");

        builder.Property(x => x.IsActive)
            .HasColumnType("boolean");

        builder.HasOne(x => x.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId);

        builder.HasOne(x => x.Media)
            .WithMany()
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
