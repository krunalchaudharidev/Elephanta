using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Application.Features.Authentication.Services;
using Elephanta.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Elephanta.Domain.Constants;

namespace Elephanta.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;
    private readonly IRoleRepository _roleRepository;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenService refreshTokenService, IConfiguration configuration, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
        _roleRepository = roleRepository;
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

        // Determine role to assign. Default to "User" if not provided.
        var roleName = string.IsNullOrWhiteSpace(req.Role) ? Roles.User : req.Role.Trim();

        // Prevent assigning privileged/forbidden roles during open registration.
        var forbidden = _configuration.GetSection("Registration:ForbiddenRoles").Get<string[]>() ?? Array.Empty<string>();
        foreach (var f in forbidden)
        {
            if (string.Equals(f, roleName, StringComparison.OrdinalIgnoreCase))
            {
                // fallback to safe default
                roleName = Roles.User;
                break;
            }
        }

        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
        {
            role = new Role { Id = Guid.NewGuid(), Name = roleName };
            role = await _roleRepository.AddAsync(role);
        }

        // Attach role to user via join entity so EF will persist the relationship when adding the user
        user.UserRoles = new[] { new UserRole { UserId = user.Id, RoleId = role.Id } };

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
