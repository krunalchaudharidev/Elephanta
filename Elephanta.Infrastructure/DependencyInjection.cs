using Microsoft.Extensions.DependencyInjection;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Infrastructure.Authentication;

namespace Elephanta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // repository registrations
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserRepository, Elephanta.Infrastructure.Repositories.UserRepository>();
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserAddressRepository, Elephanta.Infrastructure.Repositories.UserAddressRepository>();

        return services;
    }
}
