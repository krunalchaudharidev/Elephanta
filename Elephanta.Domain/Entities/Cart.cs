using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? SessionId { get; set; }

    public Guid? OfferId { get; set; }

    public string? CouponCode { get; set; }

    public User? User { get; set; }

    public Offer? Offer { get; set; }
}
