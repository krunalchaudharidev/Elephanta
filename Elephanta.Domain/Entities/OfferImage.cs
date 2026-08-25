using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class OfferImage : BaseEntity
{
    public Guid OfferId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation
    public Offer Offer { get; set; } = null!;
}
