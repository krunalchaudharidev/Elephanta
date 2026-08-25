using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Authentication.Services;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
[ApiExplorerSettings(GroupName = "User")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserAddressService _addressService;

    public UserController(IUserService userService, IUserAddressService addressService)
    {
        _userService = userService;
        _addressService = addressService;
    }

    // Validate password strength. Returns list of error messages (empty if valid).
    private static List<string> ValidatePassword(string? password)
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(password))
        {
            errors.Add("Password is required.");
            return errors;
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit.");

        const string special = "!@#$%^&*()-_+=[]{}|;:'\",.<>?/~`\\";
        if (!password.Any(c => special.Contains(c)))
            errors.Add("Password must contain at least one special character (e.g. !@#$%).");

        return errors;
    }

    /// <summary>
    /// Update user profile. Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest req)
    {
        // get user id from token 'sub' claim
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        // apply updates
        if (req.FirstName != null) user.FirstName = req.FirstName;
        if (req.MiddleName != null) user.MiddleName = req.MiddleName;
        if (req.LastName != null) user.LastName = req.LastName;
        if (req.PhoneNumber != null) user.PhoneNumber = req.PhoneNumber;

        user.UpdatedAt = DateTime.UtcNow;

        await _userService.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Change password for the authenticated user. Requires current (old) password.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (string.IsNullOrWhiteSpace(req.OldPassword)) return BadRequest(new { message = "OldPassword is required" });

        if (!PasswordHasher.Verify(req.OldPassword, user.PasswordHash)) return Unauthorized(new { message = "Current password is incorrect" });
        var validationErrors = ValidatePassword(req.NewPassword);
        if (validationErrors.Any()) return BadRequest(new { errors = validationErrors });

        user.PasswordHash = PasswordHasher.Hash(req.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userService.UpdateAsync(user);
        return NoContent();
    }

    /// <summary>
    /// Reset another user's password (admin only).
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPut("users/{id}/password")]
    public async Task<IActionResult> AdminResetPassword(Guid id, [FromBody] ChangePasswordRequest req)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        if (string.IsNullOrWhiteSpace(req.NewPassword)) return BadRequest(new { message = "NewPassword is required" });

        var validationErrors = ValidatePassword(req.NewPassword);
        if (validationErrors.Any()) return BadRequest(new { errors = validationErrors });

        user.PasswordHash = PasswordHasher.Hash(req.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userService.UpdateAsync(user);
        return NoContent();
    }

    /// <summary>
    /// Create a new address for the authenticated user. Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateUserAddressRequest req)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var addr = await _addressService.CreateAddressAsync(userId, req);

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

    /// <summary>
    /// Get addresses for the authenticated user. Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var list = await _addressService.GetByUserAsync(userId);
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

    /// <summary>
    /// Get specific address by id for the authenticated user. Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpGet("addresses/{id}")]
    public async Task<IActionResult> GetAddress(Guid id)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var a = await _addressService.GetByIdAsync(id);
        if (a == null || a.UserId != userId) return NotFound();

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

    /// <summary>
    /// Update an address for the authenticated user. Requires authenticated User or Admin role.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateUserAddressRequest req)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var a = await _addressService.GetByIdAsync(id);
        if (a == null || a.UserId != userId) return NotFound();

        await _addressService.UpdateAddressAsync(userId, id, req);

        return NoContent();
    }
}
