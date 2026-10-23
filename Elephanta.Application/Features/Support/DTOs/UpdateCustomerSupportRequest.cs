namespace Elephanta.Application.Features.Support.DTOs;

public class UpdateCustomerSupportRequest
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? IsResolved { get; set; }
}
