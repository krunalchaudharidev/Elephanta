using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Catalog.DTOs;

public class ProductRequest
{
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string Slug { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string SKU { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? CompareAtPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;

    [Required]
    public Guid CategoryId { get; set; }
}

public class ProductResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string SKU { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    public Guid CategoryId { get; set; }

    public List<Guid> ImageIds { get; set; } = new List<Guid>();

    public int ReviewCount { get; set; }
}
