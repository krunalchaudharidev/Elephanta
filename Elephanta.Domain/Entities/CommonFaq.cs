using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class CommonFaq : BaseEntity
{
    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}
