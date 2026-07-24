using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using SportsGurukul.Api;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Infrastructure;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Infrastructure.Caching;
using SportsGurukul.Infrastructure.Email;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories;
using SportsGurukul.Infrastructure.Storage;
using SportsGurukul.IntegrationTests.Infrastructure;

namespace SportsGurukul.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    private string _connectionString = string.Empty;

    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            var appDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationDbContext));
            if (appDbContextDescriptor is not null)
                services.Remove(appDbContextDescriptor);

            var unitOfWorkDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUnitOfWork));
            if (unitOfWorkDescriptor is not null)
                services.Remove(unitOfWorkDescriptor);

            var infrastructureDescriptors = services.Where(s =>
                s.ServiceType == typeof(IRepository<>)).ToList();
            foreach (var d in infrastructureDescriptors)
                services.Remove(d);

            var specificRepoDescriptors = services.Where(s =>
                s.ServiceType == typeof(IUserRepository) ||
                s.ServiceType == typeof(IRefreshTokenRepository) ||
                s.ServiceType == typeof(IEmailVerificationTokenRepository) ||
                s.ServiceType == typeof(IPasswordResetTokenRepository) ||
                s.ServiceType == typeof(IRoleRepository) ||
                s.ServiceType == typeof(IPermissionRepository) ||
                s.ServiceType == typeof(IUserRoleRepository) ||
                s.ServiceType == typeof(IUserProfileRepository) ||
                s.ServiceType == typeof(IFileRepository) ||
                s.ServiceType == typeof(IFileStorageService) ||
                s.ServiceType == typeof(IEmailService) ||
                s.ServiceType == typeof(ICacheService) ||
                s.ServiceType == typeof(IAthleteRepository) ||
                s.ServiceType == typeof(ISportRepository) ||
                s.ServiceType == typeof(IAchievementRepository) ||
                s.ServiceType == typeof(IAthleteDocumentRepository) ||
                s.ServiceType == typeof(ISavedSearchRepository) ||
                s.ServiceType == typeof(IRecentSearchRepository) ||
                s.ServiceType == typeof(ICoachSearchRepository)).ToList();
            foreach (var d in specificRepoDescriptors)
                services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_connectionString));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddInfrastructureServicesForTest();

            services.Configure<StorageOptions>(opts =>
            {
                opts.Provider = StorageProvider.Local;
                opts.BasePath = Path.Combine(Path.GetTempPath(), "sportsgurukul_test_uploads");
            });

            services.Configure<JwtOptions>(opts =>
            {
                opts.Issuer = JwtTestHelper.TestIssuer;
                opts.Audience = JwtTestHelper.TestAudience;
                opts.SigningKey = JwtTestHelper.TestSigningKey;
            });
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var tableNames = dbContext.Model.GetEntityTypes()
            .Select(t => t.GetTableName()!)
            .Where(n => !string.IsNullOrEmpty(n) && n != "__EFMigrationsHistory")
            .Distinct()
            .ToList();

        foreach (var tableName in tableNames)
        {
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    $"TRUNCATE TABLE \"{tableName}\" CASCADE");
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01")
            {
                // Table doesn't exist yet, skip it
            }
        }
    }

    public new async Task DisposeAsync()
    {
    }
}

public static class InfrastructureServiceExtensionsForTest
{
    public static IServiceCollection AddInfrastructureServicesForTest(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAthleteRepository, AthleteRepository>();
        services.AddScoped<ISportRepository, SportRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IAthleteDocumentRepository, AthleteDocumentRepository>();
        services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();
        services.AddScoped<IRecentSearchRepository, RecentSearchRepository>();
        services.AddScoped<ICoachRepository, CoachRepository>();
        services.AddScoped<ICoachCertificationRepository, CoachCertificationRepository>();
        services.AddScoped<ICoachAvailabilityRepository, CoachAvailabilityRepository>();
        services.AddScoped<ICoachDocumentRepository, CoachDocumentRepository>();
        services.AddScoped<ICoachSearchRepository, CoachSearchRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<LocalStorageService>();
        services.AddScoped<AzureBlobStorageService>();
        services.AddScoped<S3StorageService>();
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
        services.AddScoped<IEmailService, SmtpEmailService>();
        return services;
    }
}
