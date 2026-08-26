using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class ProductFaqConfiguration : IEntityTypeConfiguration<ProductFaq>
{
    public void Configure(EntityTypeBuilder<ProductFaq> builder)
    {
        builder.ToTable("ProductFaqs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Answer)
            .HasColumnType("text");

        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.Faqs)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
