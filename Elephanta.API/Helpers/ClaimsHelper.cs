using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Elephanta.API.Helpers;

public static class ClaimsHelper
{
    /// <summary>
    /// Try to extract a GUID user id from common claim names in the provided ClaimsPrincipal.
    /// Returns null when no suitable GUID claim is found.
    /// </summary>
    public static Guid? GetUserIdFromClaims(ClaimsPrincipal? user)
    {
        if (user == null) return null;

        var candidates = new[]
        {
            user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            user.FindFirst("id")?.Value,
            user.FindFirst("userId")?.Value
        };

        foreach (var c in candidates)
        {
            if (!string.IsNullOrWhiteSpace(c) && Guid.TryParse(c, out var g))
                return g;
        }

        return null;
    }
}
