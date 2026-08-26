namespace Elephanta.Application.Features.Support.DTOs;

public class UpdateCustomerSupportRequest
{
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? IsResolved { get; set; }
}
