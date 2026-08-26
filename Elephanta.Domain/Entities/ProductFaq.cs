using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class ProductFaq : BaseEntity
{
    public Guid ProductId { get; set; }

    public string Question { get; set; } = null!;

    public string? Answer { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
