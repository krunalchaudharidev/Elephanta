using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Status { get; set; } = "Pending";

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ICollection<OrderAddress> Addresses { get; set; } = new List<OrderAddress>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
