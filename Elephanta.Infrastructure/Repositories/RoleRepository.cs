using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ElephantaDbContext _db;

    public RoleRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Role> AddAsync(Role role)
    {
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return role;
    }
}
