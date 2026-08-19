using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class ProductReview : BaseEntity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public Guid? UserId { get; set; }
}
