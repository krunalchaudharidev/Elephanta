using System;
using System.Collections.Generic;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Parent category
    public Guid? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }

    // Navigation
    public ICollection<Category> SubCategories { get; set; }
        = new List<Category>();

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
