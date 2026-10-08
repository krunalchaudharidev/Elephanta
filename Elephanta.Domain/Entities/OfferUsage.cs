using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class OfferUsage : BaseEntity
{
    public Guid OfferId { get; set; }

    // Optional: the user who used the offer
    public Guid? UserId { get; set; }

    // Optional: related order id when the offer was used
    public Guid? OrderId { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime UsedAt { get; set; }

    // Navigation
    public Offer? Offer { get; set; }
    public User? User { get; set; }
}
