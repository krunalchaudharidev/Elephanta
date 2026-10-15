using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Persistence.Configurations;

public class WebsiteSettingConfiguration : IEntityTypeConfiguration<WebsiteSetting>
{
    public void Configure(EntityTypeBuilder<WebsiteSetting> builder)
    {
        builder.ToTable("WebsiteSettings");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.SiteName).IsRequired().HasMaxLength(255);
        builder.Property(w => w.FacebookURL).HasMaxLength(500);
        builder.Property(w => w.InstagramURL).HasMaxLength(500);
        builder.Property(w => w.PhoneNo).HasMaxLength(50);
        builder.Property(w => w.Email).HasMaxLength(255);
        builder.Property(w => w.FreeShippingOnOrdersAbove).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(w => w.Address).HasMaxLength(1000);
        builder.Property(w => w.IsMaintenanceMode).HasDefaultValue(false);

        // Seed a single default record. The Id is fixed so migrations won't create duplicates.
        builder.HasData(new WebsiteSetting
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            SiteName = "My Site",
            FacebookURL = null,
            InstagramURL = null,
            PhoneNo = null,
            Email = null,
            FreeShippingOnOrdersAbove = 0m,
            Address = null,
            IsMaintenanceMode = false,
            CreatedAt = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
