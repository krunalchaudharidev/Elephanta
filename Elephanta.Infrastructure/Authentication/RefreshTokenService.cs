using System.Security.Cryptography;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Authentication;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ElephantaDbContext _db;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(ElephantaDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
    {
        var expiresDaysText = _configuration["Jwt:RefreshTokenExpiresDays"];
        var expiresDays = 7;
        if (!string.IsNullOrEmpty(expiresDaysText) && int.TryParse(expiresDaysText, out var parsed))
        {
            expiresDays = parsed;
        }

        var token = CreateRandomToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiresDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
    {
        var rt = await _db.RefreshTokens.FindAsync(Guid.TryParse(token, out var gid) ? gid : Guid.Empty);
        // token is stored as string, not guid; try lookup by token value
        if (rt == null)
        {
            rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        if (rt == null) return null;
        if (rt.IsRevoked) return null;
        if (rt.ExpiresAt <= DateTime.UtcNow) return null;

        return rt;
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _db.RefreshTokens.Update(token);
        await _db.SaveChangesAsync();
    }

    private static string CreateRandomToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
