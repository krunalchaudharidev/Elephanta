using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public Guid? ProductId { get; set; }

    public Product? Product { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductSku { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal LineTotal { get; set; }
}
