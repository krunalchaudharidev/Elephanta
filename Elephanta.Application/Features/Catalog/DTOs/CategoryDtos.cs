using System;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Catalog.DTOs;

public class CategoryRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? ParentCategoryId { get; set; }
}

public class CategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid? ParentCategoryId { get; set; }
}
