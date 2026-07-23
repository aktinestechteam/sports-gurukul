using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Infrastructure.Email;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories;
using SportsGurukul.Infrastructure.Storage;

namespace SportsGurukul.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        services.AddScoped<IFileStorageService>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<StorageOptions>>().Value;
            return options.Provider switch
            {
                StorageProvider.Azure => sp.GetRequiredService<AzureBlobStorageService>(),
                StorageProvider.S3 => sp.GetRequiredService<S3StorageService>(),
                _ => sp.GetRequiredService<LocalStorageService>()
            };
        });
        services.AddScoped<LocalStorageService>();
        services.AddScoped<AzureBlobStorageService>();
        services.AddScoped<S3StorageService>();

        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
