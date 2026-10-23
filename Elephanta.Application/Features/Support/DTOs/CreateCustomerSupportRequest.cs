namespace Elephanta.Application.Features.Support.DTOs;

public class CreateCustomerSupportRequest
{
    public string Name { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
