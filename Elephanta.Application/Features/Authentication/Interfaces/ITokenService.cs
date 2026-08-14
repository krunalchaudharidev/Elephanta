using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
