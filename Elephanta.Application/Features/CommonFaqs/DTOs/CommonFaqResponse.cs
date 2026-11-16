namespace Elephanta.Application.Features.CommonFaqs.DTOs;

public class CommonFaqResponse
{
    public Guid Id { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
