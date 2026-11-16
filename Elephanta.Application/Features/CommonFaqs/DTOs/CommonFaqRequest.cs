using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.CommonFaqs.DTOs;

public class CommonFaqRequest
{
    [Required]
    [StringLength(500)]
    public string Question { get; set; } = null!;

    [Required]
    public string Answer { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}
