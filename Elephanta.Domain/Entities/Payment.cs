using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string? ProviderReference { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime? PaidAt { get; set; }

    public string? FailureReason { get; set; }
}
