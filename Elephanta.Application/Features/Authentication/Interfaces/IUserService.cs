using System;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IUserService
{
    Task<PagedResult<User>> GetCustomersAsync(
        int pageNumber,
        int pageSize,
        string? name,
        string? email,
        string? phoneNumber,
        bool sortDescending);

    Task<User> CreateAsync(User user);

    Task<User?> GetByIdAsync(Guid id);

    Task UpdateAsync(User user);
}
