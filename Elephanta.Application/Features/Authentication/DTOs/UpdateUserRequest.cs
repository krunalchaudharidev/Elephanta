namespace Elephanta.Application.Features.Authentication.DTOs;

public class UpdateUserRequest
{
    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }
}
