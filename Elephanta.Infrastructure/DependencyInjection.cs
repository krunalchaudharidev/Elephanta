using Microsoft.Extensions.DependencyInjection;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Infrastructure.Authentication;
using Elephanta.Infrastructure.Repositories;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Infrastructure.Services;

namespace Elephanta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAddressService, UserAddressService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
