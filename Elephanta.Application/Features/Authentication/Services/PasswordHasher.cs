using System;
using System.Security.Cryptography;

namespace Elephanta.Application.Features.Authentication.Services;

public static class PasswordHasher
{
    // PBKDF2
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var combined = new byte[1 + salt.Length + hash.Length];
        combined[0] = 0; // version
        Buffer.BlockCopy(salt, 0, combined, 1, salt.Length);
        Buffer.BlockCopy(hash, 0, combined, 1 + salt.Length, hash.Length);

        return Convert.ToBase64String(combined);
    }

    public static bool Verify(string password, string hashed)
    {
        try
        {
            var combined = Convert.FromBase64String(hashed);
            if (combined[0] != 0) return false;

            var salt = new byte[16];
            Buffer.BlockCopy(combined, 1, salt, 0, salt.Length);
            var hash = new byte[32];
            Buffer.BlockCopy(combined, 1 + salt.Length, hash, 0, hash.Length);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
        catch
        {
            return false;
        }
    }
}
