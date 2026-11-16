using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Common;
using Elephanta.Domain.Constants;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ElephantaDbContext _db;

    public UserRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<User>> GetCustomersAsync(
        int pageNumber,
        int pageSize,
        string? name,
        string? email,
        string? phoneNumber,
        bool sortDescending)
    {
        var customersQuery = _db.Users
            .AsNoTracking()
            .Where(user => user.UserRoles!.Any(userRole => userRole.Role!.Name == Roles.User)
                && !user.UserRoles!.Any(userRole => userRole.Role!.Name == Roles.Admin));

        if (!string.IsNullOrWhiteSpace(name))
        {
            foreach (var namePart in name.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var namePattern = $"%{namePart}%";
                customersQuery = customersQuery.Where(user =>
                    EF.Functions.ILike(user.FirstName ?? string.Empty, namePattern)
                    || EF.Functions.ILike(user.MiddleName ?? string.Empty, namePattern)
                    || EF.Functions.ILike(user.LastName ?? string.Empty, namePattern));
            }
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailPattern = $"%{email.Trim()}%";
            customersQuery = customersQuery.Where(user => EF.Functions.ILike(user.Email, emailPattern));
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phonePattern = $"%{phoneNumber.Trim()}%";
            customersQuery = customersQuery.Where(user => EF.Functions.ILike(user.PhoneNumber ?? string.Empty, phonePattern));
        }

        var totalCount = await customersQuery.CountAsync();

        var orderedCustomers = sortDescending
            ? customersQuery
                .OrderByDescending(user => user.FirstName)
                .ThenByDescending(user => user.MiddleName)
                .ThenByDescending(user => user.LastName)
                .ThenBy(user => user.Email)
            : customersQuery
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.MiddleName)
                .ThenBy(user => user.LastName)
                .ThenBy(user => user.Email);

        var customers = await orderedCustomers
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = customers,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> AnyByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email)
    {
        return await _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id)
    {
        return await _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
