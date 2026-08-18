using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IUserAddressRepository
{
    Task AddAsync(UserAddress address);

    Task<UserAddress?> GetByIdAsync(Guid id);

    Task<List<UserAddress>> GetByUserAsync(Guid userId);

    Task UpdateAsync(UserAddress address);
}
