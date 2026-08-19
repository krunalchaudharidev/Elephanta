using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class UserAddressService : IUserAddressService
{
    private readonly IUserAddressRepository _repo;

    public UserAddressService(IUserAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserAddress> CreateAddressAsync(Guid userId, CreateUserAddressRequest req)
    {
        if (req.IsPrimary)
        {
            var existing = await _repo.GetByUserAsync(userId);
            foreach (var e in existing.Where(a => a.IsPrimary))
            {
                e.IsPrimary = false;
                await _repo.UpdateAsync(e);
            }
        }

        var addr = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AddressLine1 = req.AddressLine1,
            AddressLine2 = req.AddressLine2,
            City = req.City,
            State = req.State,
            PostalCode = req.PostalCode,
            Country = req.Country,
            IsPrimary = req.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(addr);
        return addr;
    }

    public async Task<List<UserAddress>> GetByUserAsync(Guid userId)
    {
        return await _repo.GetByUserAsync(userId);
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task UpdateAddressAsync(Guid userId, Guid id, UpdateUserAddressRequest req)
    {
        var a = await _repo.GetByIdAsync(id);
        if (a == null || a.UserId != userId) throw new InvalidOperationException("Address not found");

        if (req.IsPrimary == true)
        {
            var existing = await _repo.GetByUserAsync(userId);
            foreach (var e in existing.Where(x => x.IsPrimary && x.Id != id))
            {
                e.IsPrimary = false;
                await _repo.UpdateAsync(e);
            }
        }

        if (req.AddressLine1 != null) a.AddressLine1 = req.AddressLine1;
        if (req.AddressLine2 != null) a.AddressLine2 = req.AddressLine2;
        if (req.City != null) a.City = req.City;
        if (req.State != null) a.State = req.State;
        if (req.PostalCode != null) a.PostalCode = req.PostalCode;
        if (req.Country != null) a.Country = req.Country;
        if (req.IsPrimary.HasValue) a.IsPrimary = req.IsPrimary.Value;

        a.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(a);
    }
}
