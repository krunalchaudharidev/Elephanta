using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string? PreviousStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public string? Note { get; set; }

    public Guid? ChangedByUserId { get; set; }
}
