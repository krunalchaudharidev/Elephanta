using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class CustomerSupportRequest : BaseEntity
{
    // Optional user association (guest or registered user)
    public Guid? UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsResolved { get; set; } = false;

    public DateTime? ResolvedAt { get; set; }

    // Navigation
    public User? User { get; set; }
}
