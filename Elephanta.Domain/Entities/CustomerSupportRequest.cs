using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class CustomerSupportRequest : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsResolved { get; set; } = false;

    public DateTime? ResolvedAt { get; set; }
}
