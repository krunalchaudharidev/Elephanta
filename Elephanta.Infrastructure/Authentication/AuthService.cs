using System;
using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Application.Features.Authentication.Services;
using Elephanta.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Elephanta.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenService refreshTokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        var exists = await _userRepository.AnyByEmailAsync(req.Email);
        if (exists) throw new InvalidOperationException("Email already in use");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            PhoneNumber = req.PhoneNumber,
            Email = req.Email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes) };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _userRepository.GetByEmailWithRolesAsync(req.Email);
        if (user == null) throw new UnauthorizedAccessException();

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash)) throw new UnauthorizedAccessException();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        // update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes) };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var existing = await _refreshTokenService.GetValidRefreshTokenAsync(refreshToken);
        if (existing == null) throw new UnauthorizedAccessException();

        await _refreshTokenService.RevokeRefreshTokenAsync(existing);

        var user = await _userRepository.GetByIdWithRolesAsync(existing.UserId);
        if (user == null) throw new UnauthorizedAccessException();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed)) expiresMinutes = parsed;

        return new AuthResponse { AccessToken = access, RefreshToken = refresh.Token, ExpiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes) };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var existing = await _refreshTokenService.GetValidRefreshTokenAsync(refreshToken);
        if (existing == null) return;
        await _refreshTokenService.RevokeRefreshTokenAsync(existing);
    }
}
