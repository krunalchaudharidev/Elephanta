using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Offer : BaseEntity
{
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

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<OfferImage> Images { get; set; } = new List<OfferImage>();
}
