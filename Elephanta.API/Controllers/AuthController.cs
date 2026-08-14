using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Infrastructure.Persistence;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.Services;
using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ElephantaDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(ElephantaDbContext db, ITokenService tokenService, IRefreshTokenService refreshTokenService, IConfiguration configuration)
    {
        _db = db;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists) return Conflict(new { message = "Email already in use" });

        var user = new User
        {
            Id = System.Guid.NewGuid(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            IsActive = true,
            CreatedAt = System.DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = System.DateTime.UtcNow.AddMinutes(expiresMinutes) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null) return Unauthorized();

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash)) return Unauthorized();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = System.DateTime.UtcNow.AddMinutes(expiresMinutes) });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var existing = await _refreshTokenService.GetValidRefreshTokenAsync(req.RefreshToken);
        if (existing == null) return Unauthorized();

        // rotate: revoke old and create new
        await _refreshTokenService.RevokeRefreshTokenAsync(existing);

        var user = await _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == existing.UserId);
        if (user == null) return Unauthorized();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = System.DateTime.UtcNow.AddMinutes(expiresMinutes) });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
    {
        var existing = await _refreshTokenService.GetValidRefreshTokenAsync(req.RefreshToken);
        if (existing == null) return BadRequest();

        await _refreshTokenService.RevokeRefreshTokenAsync(existing);
        return NoContent();
    }
}
