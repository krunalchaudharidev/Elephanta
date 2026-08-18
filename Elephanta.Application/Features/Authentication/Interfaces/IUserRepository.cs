using System;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IUserRepository
{
    Task<bool> AnyByEmailAsync(string email);

    Task AddAsync(User user);

    Task<User?> GetByEmailWithRolesAsync(string email);

    Task<User?> GetByIdWithRolesAsync(Guid id);

    Task<User?> GetByIdAsync(Guid id);

    Task UpdateAsync(User user);
}
