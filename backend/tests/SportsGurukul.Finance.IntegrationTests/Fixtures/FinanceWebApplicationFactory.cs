using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SportsGurukul.Api;
using SportsGurukul.Finance.IntegrationTests.Seed;
using SportsGurukul.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Fixtures;

public sealed class FinanceWebApplicationFactory
    : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private bool _disposed;

    public FinanceWebApplicationFactory()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("sportsgurukul_finance_test")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .Build();
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            await _container.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _container.GetConnectionString();

        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            var ctxDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));
            if (ctxDescriptor is not null) services.Remove(ctxDescriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            var policyDescriptors = services
                .Where(d => d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericTypeDefinition() == typeof(IRateLimiterPolicy<>))
                .ToList();
            foreach (var pd in policyDescriptors)
                services.Remove(pd);

            services.PostConfigure<RateLimiterOptions>(options =>
            {
                options.GlobalLimiter = null;
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = FinanceTestIds.JwtIssuer,
                    ValidAudience = FinanceTestIds.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(FinanceTestIds.JwtSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString,
                ["Jwt:Issuer"] = FinanceTestIds.JwtIssuer,
                ["Jwt:Audience"] = FinanceTestIds.JwtAudience,
                ["Jwt:SigningKey"] = FinanceTestIds.JwtSigningKey,
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"] = "30",
                ["Storage:Provider"] = "Local",
                ["Storage:BasePath"] = "test-uploads"
            });
        });
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
            }
        }
    }
}
