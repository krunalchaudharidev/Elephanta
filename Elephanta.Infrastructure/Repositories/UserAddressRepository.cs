using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class UserAddressRepository : IUserAddressRepository
{
    private readonly ElephantaDbContext _db;

    public UserAddressRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(UserAddress address)
    {
        _db.UserAddresses.Add(address);
        await _db.SaveChangesAsync();
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id)
    {
        return await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<UserAddress>> GetByUserAsync(Guid userId)
    {
        return await _db.UserAddresses.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task UpdateAsync(UserAddress address)
    {
        _db.UserAddresses.Update(address);
        await _db.SaveChangesAsync();
    }
}
