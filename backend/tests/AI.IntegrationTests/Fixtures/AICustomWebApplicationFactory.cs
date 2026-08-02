using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SportsGurukul.Api;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace AI.IntegrationTests.Fixtures;

public sealed class AICustomWebApplicationFactory
    : WebApplicationFactory<ApiMarker>, IClassFixture<AICustomWebApplicationFactory>
{
    private static readonly PostgreSqlContainer Container;
    private static bool _databaseInitialized;
    private static readonly object DatabaseLock = new();
    private bool _disposed;

    public const string TestJwtSigningKey = "AI-Integration-Test-Secret-Key-That-Is-At-Least-32-Chars!!";
    public const string TestJwtIssuer = "SportsGurukul.AI.Test";
    public const string TestJwtAudience = "SportsGurukul.AI.Test";

    static AICustomWebApplicationFactory()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("sportsgurukul_ai_test")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .Build();

        Container.StartAsync().GetAwaiter().GetResult();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = Container.GetConnectionString();

        builder.UseEnvironment("Development");
        builder.UseSetting("https_port", "");

        builder.ConfigureServices(services =>
        {
            RemoveService<DbContextOptions<ApplicationDbContext>>(services);
            RemoveService<ApplicationDbContext>(services);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            var policyDescriptors = services
                .Where(d => d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericTypeDefinition() == typeof(IRateLimiterPolicy<>))
                .ToList();
            foreach (var descriptor in policyDescriptors)
                services.Remove(descriptor);

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
                    ValidIssuer = TestJwtIssuer,
                    ValidAudience = TestJwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(TestJwtSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Headers.TryGetValue("X-Test-Claims", out var header))
                        {
                            var claims = DecodeTestClaims(header.ToString());
                            if (claims.Count > 0)
                                context.Token = CreateTestToken(claims);
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.PostConfigure<JwtOptions>(options =>
            {
                options.Issuer = TestJwtIssuer;
                options.Audience = TestJwtAudience;
                options.SigningKey = TestJwtSigningKey;
                options.AccessTokenExpirationMinutes = 60;
            });
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString,
                ["Jwt:Issuer"] = TestJwtIssuer,
                ["Jwt:Audience"] = TestJwtAudience,
                ["Jwt:SigningKey"] = TestJwtSigningKey,
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"] = "30",
                ["Storage:Provider"] = "Local",
                ["Storage:BasePath"] = "test-uploads"
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        lock (DatabaseLock)
        {
            if (!_databaseInitialized)
            {
                using var scope = host.Services.CreateScope();
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var createScript = dbContext.Database.GenerateCreateScript();
                ExecuteSqlScript(dbContext, StripSeedInserts(createScript));
                SeedReferenceData(dbContext);
                _databaseInitialized = true;
            }
        }

        return host;
    }

    private static void SeedReferenceData(ApplicationDbContext dbContext)
    {
        if (dbContext.Roles.Any())
            return;

        SeedRolesAndUsers(dbContext);
        SeedAiCatalog(dbContext);
    }

    private static void SeedRolesAndUsers(ApplicationDbContext dbContext)
    {
        var roles = new List<Role>
        {
            new() { Id = AITestIds.SuperAdminRoleId, Name = "SuperAdmin", RoleType = RoleType.SuperAdmin },
            new() { Id = AITestIds.AdminRoleId, Name = "Admin", RoleType = RoleType.Admin },
            new() { Id = AITestIds.CoachRoleId, Name = "Coach", RoleType = RoleType.Coach },
            new() { Id = AITestIds.AthleteRoleId, Name = "Athlete", RoleType = RoleType.Athlete }
        };
        dbContext.Roles.AddRange(roles);

        var users = new List<User>
        {
            new()
            {
                Id = AITestIds.AdminUserId,
                Email = "ai-admin@test.com",
                PhoneNumber = "+1000000101",
                PasswordHash = "test-hash",
                FullName = "AI Admin",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = AITestIds.CoachUserId,
                Email = "ai-coach@test.com",
                PhoneNumber = "+1000000102",
                PasswordHash = "test-hash",
                FullName = "AI Coach",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = AITestIds.AthleteUserId,
                Email = "ai-athlete@test.com",
                PhoneNumber = "+1000000103",
                PasswordHash = "test-hash",
                FullName = "AI Athlete",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            }
        };
        dbContext.Users.AddRange(users);

        dbContext.UserRoles.AddRange(new List<UserRole>
        {
            new() { UserId = AITestIds.AdminUserId, RoleId = AITestIds.AdminRoleId, AssignedAt = DateTime.UtcNow },
            new() { UserId = AITestIds.CoachUserId, RoleId = AITestIds.CoachRoleId, AssignedAt = DateTime.UtcNow },
            new() { UserId = AITestIds.AthleteUserId, RoleId = AITestIds.AthleteRoleId, AssignedAt = DateTime.UtcNow }
        });
    }

    private static void SeedAiCatalog(ApplicationDbContext dbContext)
    {
        var openAiProvider = new AIProvider
        {
            Id = AITestIds.OpenAiProviderId,
            Name = "OpenAI",
            Description = "OpenAI provider",
            Type = AIProviderType.OpenAI,
            IsActive = true,
            MaxRetries = 3,
            TimeoutSeconds = 60,
            CostPerToken = 0.000003m
        };
        var googleProvider = new AIProvider
        {
            Id = AITestIds.GoogleProviderId,
            Name = "Google",
            Description = "Google provider",
            Type = AIProviderType.Google,
            IsActive = true,
            MaxRetries = 2,
            TimeoutSeconds = 60
        };
        dbContext.AIProviders.AddRange(openAiProvider, googleProvider);

        dbContext.AIModels.AddRange(
            new AIModel
            {
                Id = AITestIds.Gpt4oModelId,
                ProviderId = AITestIds.OpenAiProviderId,
                Name = "gpt-4o",
                DisplayName = "GPT-4o",
                Description = "Flagship multimodal model",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.FunctionCalling | AIModelCapability.Vision | AIModelCapability.Reasoning,
                Status = AIModelStatus.Active,
                MaxTokens = 16384,
                MaxContextLength = 128000,
                CostPerInputToken = 0.000005m,
                CostPerOutputToken = 0.000015m,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                SupportsFunctionCalling = true,
                SupportsVision = true,
                SupportsEmbeddings = false,
                ModelVersion = "2024-08-06"
            },
            new AIModel
            {
                Id = AITestIds.EmbeddingModelId,
                ProviderId = AITestIds.OpenAiProviderId,
                Name = "text-embedding-3-small",
                DisplayName = "text-embedding-3-small",
                Description = "Embedding model",
                Capabilities = AIModelCapability.Embedding,
                Status = AIModelStatus.Active,
                MaxTokens = 8191,
                MaxContextLength = 8191,
                DefaultTemperature = 0.0,
                SupportsStreaming = false,
                SupportsFunctionCalling = false,
                SupportsVision = false,
                SupportsEmbeddings = true
            },
            new AIModel
            {
                Id = AITestIds.GeminiModelId,
                ProviderId = AITestIds.GoogleProviderId,
                Name = "gemini-1.5-pro",
                DisplayName = "Gemini 1.5 Pro",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.Reasoning,
                Status = AIModelStatus.Preview,
                DefaultTemperature = 0.4,
                SupportsStreaming = true,
                SupportsFunctionCalling = true
            },
            new AIModel
            {
                Id = AITestIds.DeprecatedModelId,
                ProviderId = AITestIds.OpenAiProviderId,
                Name = "gpt-3.5-turbo",
                DisplayName = "GPT-3.5 Turbo",
                Capabilities = AIModelCapability.TextGeneration,
                Status = AIModelStatus.Deprecated,
                DefaultTemperature = 0.7,
                SupportsStreaming = true
            });

        dbContext.ToolDefinitions.AddRange(
            new ToolDefinition
            {
                Id = AITestIds.SearchToolId,
                Name = "knowledge_search",
                Description = "Search the knowledge base",
                Type = ToolType.Function,
                Status = ToolStatus.Active,
                RequiresApproval = false,
                TimeoutSeconds = 30
            },
            new ToolDefinition
            {
                Id = AITestIds.NotificationToolId,
                Name = "send_notification",
                Description = "Send a notification",
                Type = ToolType.Function,
                Status = ToolStatus.Active,
                RequiresApproval = true,
                TimeoutSeconds = 15
            });

        dbContext.SaveChanges();
    }

    private static List<Claim> DecodeTestClaims(string headerValue)
    {
        var claims = new List<Claim>();
        if (string.IsNullOrWhiteSpace(headerValue))
            return claims;

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue));
        using var doc = JsonDocument.Parse(json);
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            var type = element.GetProperty("Type").GetString();
            var value = element.GetProperty("Value").GetString();
            if (!string.IsNullOrEmpty(type) && value is not null)
                claims.Add(new Claim(type, value));
        }
        return claims;
    }

    private static string CreateTestToken(IEnumerable<Claim> claims)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtSigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestJwtIssuer,
            audience: TestJwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string StripSeedInserts(string script)
    {
        var lines = script.Split('\n');
        var firstInsert = Array.FindIndex(lines, line => line.TrimStart().StartsWith("INSERT INTO", StringComparison.OrdinalIgnoreCase));
        return firstInsert < 0 ? script : string.Join('\n', lines.Take(firstInsert));
    }

    private static void ExecuteSqlScript(ApplicationDbContext dbContext, string script)
    {
        var connection = dbContext.Database.GetDbConnection();
        var wasOpen = connection.State == System.Data.ConnectionState.Open;
        if (!wasOpen)
            connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = script;
        command.ExecuteNonQuery();

        if (!wasOpen)
            connection.Close();
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor is not null)
            services.Remove(descriptor);
    }

    public new void Dispose()
    {
        if (!_disposed)
        {
            Container.DisposeAsync().AsTask().GetAwaiter().GetResult();
            _disposed = true;
        }
    }
}
