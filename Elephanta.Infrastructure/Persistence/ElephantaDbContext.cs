using Microsoft.EntityFrameworkCore;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence.Configurations;
using Elephanta.Domain.Common;

namespace Elephanta.Infrastructure.Persistence;

// Create a migration
// dotnet ef migrations add InitialCreate --project Elephanta.Infrastructure --startup-project Elephanta.API

// Update the database
// dotnet ef database update --project Elephanta.Infrastructure --startup-project Elephanta.API

public class ElephantaDbContext : DbContext
{
    public ElephantaDbContext(DbContextOptions<ElephantaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<UserRole> UserRoles { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public DbSet<UserAddress> UserAddresses { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<ProductImage> ProductImages { get; set; } = null!;

    public DbSet<ProductReview> ProductReviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.Entity<UserAddress>();
        modelBuilder.Entity<Category>();
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<ProductImage>();
        modelBuilder.Entity<ProductReview>();
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAt == default)
                    entry.Entity.CreatedAt = utcNow;

                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }
}
