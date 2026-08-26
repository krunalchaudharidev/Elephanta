using System;

namespace Elephanta.Application.Features.Support.DTOs;

public class CustomerSupportRequestResponse
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
