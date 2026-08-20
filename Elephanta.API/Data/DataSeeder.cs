using System;
using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.Services;
using Elephanta.Domain.Constants;
using Elephanta.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elephanta.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration config, string adminPassword)
    {
        var roleRepo = serviceProvider.GetRequiredService<IRoleRepository>();
        var userRepo = serviceProvider.GetRequiredService<IUserRepository>();

        var seedRoles = config.GetSection("Registration:SeedRoles").Get<string[]>() ?? new[] { Roles.Admin, Roles.User };
        foreach (var seed in seedRoles)
        {
            var existing = await roleRepo.GetByNameAsync(seed);
            if (existing == null)
            {
                await roleRepo.AddAsync(new Role { Id = Guid.NewGuid(), Name = seed });
            }
        }

        // Admin user seeding using provided static password
        var adminEmail = config["Registration:AdminUser:Email"] ?? "admin@elephanta.local";
        var adminFirstName = config["Registration:AdminUser:FirstName"] ?? "Admin";
        var adminLastName = config["Registration:AdminUser:LastName"] ?? "User";

        if (!string.IsNullOrWhiteSpace(adminPassword))
        {
            var existsUser = await userRepo.AnyByEmailAsync(adminEmail);
            if (!existsUser)
            {
                var adminRole = await roleRepo.GetByNameAsync(Roles.Admin);
                if (adminRole == null)
                {
                    adminRole = await roleRepo.AddAsync(new Role { Id = Guid.NewGuid(), Name = Roles.Admin });
                }

                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    Email = adminEmail,
                    PasswordHash = PasswordHasher.Hash(adminPassword),
                    IsActive = true
                };

                adminUser.UserRoles = new[] { new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id } };

                await userRepo.AddAsync(adminUser);
            }
        }
    }
}
