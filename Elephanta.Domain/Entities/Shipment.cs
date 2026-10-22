using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Shipment : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string? Carrier { get; set; }

    public string? TrackingNumber { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
}
