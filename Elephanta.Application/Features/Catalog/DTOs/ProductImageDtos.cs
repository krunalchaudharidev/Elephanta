using System;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Catalog.DTOs;

public class ProductImageRequest
{
    [Required]
    [Url]
    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}

public class ProductImageResponse
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }
}
