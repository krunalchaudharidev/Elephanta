using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    // CreatedAt is inherited from BaseEntity

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked { get; set; }
}
