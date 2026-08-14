using Microsoft.EntityFrameworkCore;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence.Configurations;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
