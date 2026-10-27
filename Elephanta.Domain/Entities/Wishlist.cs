using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Wishlist : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;
}
