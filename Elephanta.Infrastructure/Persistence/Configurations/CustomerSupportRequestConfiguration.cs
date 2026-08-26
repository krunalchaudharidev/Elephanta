using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class CustomerSupportRequestConfiguration : IEntityTypeConfiguration<CustomerSupportRequest>
{
    public void Configure(EntityTypeBuilder<CustomerSupportRequest> builder)
    {
        builder.ToTable("CustomerSupportRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Message)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.IsResolved).HasDefaultValue(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
