using System;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Catalog.DTOs;

public class ProductReviewRequest
{
    [Range(1,5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}

public class ProductReviewResponse
{
    public Guid Id { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public Guid? UserId { get; set; }
}
