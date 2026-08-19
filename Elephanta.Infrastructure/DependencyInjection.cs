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
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserRepository, Elephanta.Infrastructure.Repositories.UserRepository>();
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserAddressRepository, Elephanta.Infrastructure.Repositories.UserAddressRepository>();
        services.AddScoped<Elephanta.Application.Features.Catalog.Interfaces.IProductRepository, Elephanta.Infrastructure.Repositories.ProductRepository>();
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IAuthService, Elephanta.Infrastructure.Authentication.AuthService>();
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserService, Elephanta.Infrastructure.Services.UserService>();
        services.AddScoped<Elephanta.Application.Features.Authentication.Interfaces.IUserAddressService, Elephanta.Infrastructure.Services.UserAddressService>();
        services.AddScoped<Elephanta.Application.Features.Catalog.Interfaces.IProductService, Elephanta.Infrastructure.Services.ProductService>();

        return services;
    }
}
