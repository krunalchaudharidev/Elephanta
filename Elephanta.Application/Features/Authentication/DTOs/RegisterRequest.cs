namespace Elephanta.Application.Features.Authentication.DTOs;

public class RegisterRequest
{
    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional role name to assign to the user during registration. Defaults to "User" when not provided.
    /// This field is subject to server-side validation and privileged roles (e.g., Admin) may be ignored.
    /// </summary>
    public string? Role { get; set; }

    public string Password { get; set; } = null!;
}
