using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IRefreshTokenService
{
    System.Threading.Tasks.Task<RefreshToken> CreateRefreshTokenAsync(System.Guid userId);

    System.Threading.Tasks.Task<RefreshToken?> GetValidRefreshTokenAsync(string token);

    System.Threading.Tasks.Task RevokeRefreshTokenAsync(RefreshToken token);
}
