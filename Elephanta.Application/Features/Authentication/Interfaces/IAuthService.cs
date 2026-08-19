using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.DTOs;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest req);

    Task<AuthResponse> LoginAsync(LoginRequest req);

    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);
}
