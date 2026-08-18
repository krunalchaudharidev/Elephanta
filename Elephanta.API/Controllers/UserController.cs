using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Domain.Entities;
using Elephanta.Application.Features.Authentication.Interfaces;
using System.Linq;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IUserAddressRepository _addressRepo;

    public UserController(IUserRepository userRepo, IUserAddressRepository addressRepo)
    {
        _userRepo = userRepo;
        _addressRepo = addressRepo;
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest req)
    {
        // get user id from token 'sub' claim
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return NotFound();

        // apply updates
        if (req.FirstName != null) user.FirstName = req.FirstName;
        if (req.MiddleName != null) user.MiddleName = req.MiddleName;
        if (req.LastName != null) user.LastName = req.LastName;
        if (req.PhoneNumber != null) user.PhoneNumber = req.PhoneNumber;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user);

        return NoContent();
    }

    [Authorize]
    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateUserAddressRequest req)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        if (req.IsPrimary)
        {
            var existingPrimary = await _addressRepo.GetByUserAsync(userId);
            foreach (var e in existingPrimary.Where(a => a.IsPrimary))
            {
                e.IsPrimary = false;
                await _addressRepo.UpdateAsync(e);
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

        await _addressRepo.AddAsync(addr);

        var resp = new UserAddressResponse
        {
            Id = addr.Id,
            AddressLine1 = addr.AddressLine1,
            AddressLine2 = addr.AddressLine2,
            City = addr.City,
            State = addr.State,
            PostalCode = addr.PostalCode,
            Country = addr.Country,
            IsPrimary = addr.IsPrimary
        };

        return CreatedAtAction(nameof(GetAddress), new { id = addr.Id }, resp);
    }

    [Authorize]
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var list = await _addressRepo.GetByUserAsync(userId);
        var resp = list.Select(a => new UserAddressResponse
        {
            Id = a.Id,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            State = a.State,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsPrimary = a.IsPrimary
        }).ToList();

        return Ok(resp);
    }

    [Authorize]
    [HttpGet("addresses/{id}")]
    public async Task<IActionResult> GetAddress(Guid id)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var a = await _addressRepo.GetByIdAsync(id);
        if (a == null || a.UserId != userId) return NotFound();
        if (a == null) return NotFound();

        var resp = new UserAddressResponse
        {
            Id = a.Id,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            State = a.State,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsPrimary = a.IsPrimary
        };

        return Ok(resp);
    }

    [Authorize]
    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateUserAddressRequest req)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var a = await _addressRepo.GetByIdAsync(id);
        if (a == null || a.UserId != userId) return NotFound();

        if (req.IsPrimary == true)
        {
            var existingPrimary = await _addressRepo.GetByUserAsync(userId);
            foreach (var e in existingPrimary.Where(x => x.IsPrimary && x.Id != id))
            {
                e.IsPrimary = false;
                await _addressRepo.UpdateAsync(e);
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

        await _addressRepo.UpdateAsync(a);

        return NoContent();
    }
}
