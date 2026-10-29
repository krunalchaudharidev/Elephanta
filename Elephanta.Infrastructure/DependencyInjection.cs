using Microsoft.Extensions.DependencyInjection;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Infrastructure.Authentication;
using Elephanta.Infrastructure.Repositories;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Application.Features.ProductFaqs.Interfaces;
using Elephanta.Application.Features.Support.Interfaces;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Infrastructure.Services;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Application.Features.WebsiteSettings.Interfaces;
using Elephanta.Application.Features.Wishlist.Interfaces;

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
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAddressService, UserAddressService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IProductFaqService, ProductFaqService>();
        services.AddScoped<ICustomerSupportService, CustomerSupportService>();
        services.AddScoped<IWebsiteSettingRepository, WebsiteSettingRepository>();
        services.AddScoped<IWebsiteSettingService, WebsiteSettingService>();

        return services;
    }
}
