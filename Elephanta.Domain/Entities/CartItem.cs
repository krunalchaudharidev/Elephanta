using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;

    public User User { get; set; } = null!;
}
