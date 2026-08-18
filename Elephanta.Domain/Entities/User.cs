using System;
using System.Collections.Generic;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class User : BaseEntity
{
    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    // Hashed password
    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserRole>? UserRoles { get; set; }

    public ICollection<RefreshToken>? RefreshTokens { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public ICollection<UserAddress>? Addresses { get; set; }
}
