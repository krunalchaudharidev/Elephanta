using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class ProductReviewImage : BaseEntity
{
    public Guid ProductReviewId { get; set; }

    public ProductReview ProductReview { get; set; } = null!;

    public Guid? MediaId { get; set; }

    public Media? Media { get; set; }

    public int DisplayOrder { get; set; }
}
