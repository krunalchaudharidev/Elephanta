using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
