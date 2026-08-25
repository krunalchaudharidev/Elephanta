using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Offers.DTOs;

public class OfferRequest
{
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public string DiscountType { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinimumOrderAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public bool IsActive { get; set; } = true;
}

public class OfferResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int UsageCount { get; set; }

    public bool IsActive { get; set; }

    public List<Guid> ImageIds { get; set; } = new List<Guid>();
}

public class OfferImageRequest
{
    [Required]
    [Url]
    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}

public class OfferImageResponse
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }
}
