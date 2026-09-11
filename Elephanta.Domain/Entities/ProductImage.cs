using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid? MediaId { get; set; }

    public Media? Media { get; set; }

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
