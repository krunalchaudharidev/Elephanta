using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class CommonFaqConfiguration : IEntityTypeConfiguration<CommonFaq>
{
    public void Configure(EntityTypeBuilder<CommonFaq> builder)
    {
        builder.ToTable("CommonFaqs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Answer)
            .HasColumnType("text");

        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
