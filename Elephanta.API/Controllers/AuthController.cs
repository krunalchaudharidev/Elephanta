using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Elephanta.Application.Features.Authentication.DTOs;
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
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenService refreshTokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var exists = await _userRepository.AnyByEmailAsync(req.Email);
        if (exists) return Conflict(new { message = "Email already in use" });

        var user = new User
        {
            Id = System.Guid.NewGuid(),
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            PhoneNumber = req.PhoneNumber,
            Email = req.Email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            IsActive = true,
            CreatedAt = System.DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

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
        var user = await _userRepository.GetByEmailWithRolesAsync(req.Email);
        if (user == null) return Unauthorized();

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash)) return Unauthorized();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        // update last login timestamp
        user.LastLoginAt = System.DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = System.DateTime.UtcNow.AddMinutes(expiresMinutes) });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var existing = await _refreshTokenService.GetValidRefreshTokenAsync(req.RefreshToken);
        if (existing == null) return Unauthorized();

        // rotate: revoke old and create new
        await _refreshTokenService.RevokeRefreshTokenAsync(existing);

        var user = await _userRepository.GetByIdWithRolesAsync(existing.UserId);
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
