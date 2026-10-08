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

    public List<Guid>? ImageIds { get; set; }

    // Optional: media id to mark as primary for this offer's images
    public Guid? PrimaryImageId { get; set; }
}

public class OfferUpdateRequest
{
    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public string? DiscountType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountValue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinimumOrderAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaximumDiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public bool? IsActive { get; set; } = true;

    public List<Guid>? ImageIds { get; set; }

    // Optional: media id to mark as primary for this offer's images
    public Guid? PrimaryImageId { get; set; }
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

    public Guid? PrimaryImageId { get; set; }
}

public class OfferImageRequest
{
    [Required]
    public Guid MediaId { get; set; }

    public bool IsPrimary { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}

public class OfferImageResponse
{
    public Guid Id { get; set; }

    public Guid? MediaId { get; set; }

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }
}
