using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IUserAddressService
{
    Task<UserAddress> CreateAddressAsync(Guid userId, CreateUserAddressRequest req);

    Task<List<UserAddress>> GetByUserAsync(Guid userId);

    Task<UserAddress?> GetByIdAsync(Guid id);

    Task UpdateAddressAsync(Guid userId, Guid id, UpdateUserAddressRequest req);
}
