using System;
using System.Collections.Generic;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string SKU { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;

    // Category
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    // Navigation
    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();

    public ICollection<ProductReview> Reviews { get; set; }
        = new List<ProductReview>();
}
