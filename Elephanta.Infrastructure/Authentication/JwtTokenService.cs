using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Elephanta.Infrastructure.Authentication;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "elephanta";
        var audience = _configuration["Jwt:Audience"] ?? "elephanta_audience";
        var expiresMinutesText = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = 60;
        if (!string.IsNullOrEmpty(expiresMinutesText) && int.TryParse(expiresMinutesText, out var parsed))
        {
            expiresMinutes = parsed;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("firstName", user.FirstName ?? string.Empty),
            new Claim("lastName", user.LastName ?? string.Empty)
        };

        // Add role claims if available
        var roleClaims = user.UserRoles?.Select(ur => new Claim(ClaimTypes.Role, ur.Role?.Name ?? string.Empty)).ToArray() ?? Array.Empty<Claim>();

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Concat(roleClaims)),
            Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
