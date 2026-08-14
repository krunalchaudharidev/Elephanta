namespace Elephanta.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}
