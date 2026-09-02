namespace Elephanta.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    // Information about the authenticated user
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    // Use Guid to match domain User.Id
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public List<string> Roles { get; set; } = new();
}
