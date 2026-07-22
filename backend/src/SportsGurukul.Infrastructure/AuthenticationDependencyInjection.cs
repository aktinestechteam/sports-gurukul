using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Infrastructure.Authentication;

namespace SportsGurukul.Infrastructure;

public static class AuthenticationDependencyInjection
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddSingleton<IPasswordHasher, ApplicationPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
