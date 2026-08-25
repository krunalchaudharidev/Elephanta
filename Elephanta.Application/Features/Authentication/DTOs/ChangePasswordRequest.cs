namespace Elephanta.Application.Features.Authentication.DTOs;

public class ChangePasswordRequest
{
    // For self-service password change: current password
    public string? OldPassword { get; set; }

    // New password to set
    public string NewPassword { get; set; } = null!;
}
