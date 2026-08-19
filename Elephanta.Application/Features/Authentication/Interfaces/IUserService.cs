using System;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(User user);

    Task<User?> GetByIdAsync(Guid id);

    Task UpdateAsync(User user);
}
