using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SportsGurukul.Api;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Infrastructure.Persistence;
using System.Threading.RateLimiting;
using Testcontainers.PostgreSql;
using Xunit;

namespace SportsGurukul.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<ApiMarker>, IClassFixture<CustomWebApplicationFactory>
{
    private static readonly PostgreSqlContainer Container;
    private static bool _databaseInitialized;
    private static readonly object DatabaseLock = new();
    private bool _disposed;

    public const string TestJwtSigningKey = "Test-Secret-Key-For-Integration-Tests-That-Is-Long-Enough-32+chars!!";
    public const string TestJwtIssuer = "SportsGurukul.Test";
    public const string TestJwtAudience = "SportsGurukul.Test";

    public static readonly Guid SuperAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AcademyRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid CoachRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid AthleteRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly Guid TestAdminUserId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid TestAcademyUserId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    public static readonly Guid TestCoachUserId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static readonly Guid TestAthleteUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    public static readonly Guid TestSportCategoryId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid TestSportId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid TestAcademyId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid TestAcademyBranchId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid TestCoachId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid TestAthleteId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static readonly Guid TestFacilityId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");

    static CustomWebApplicationFactory()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("sportsgurukul_test")
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
            {
                services.Remove(descriptor);
            }

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

                dbContext.Database.Migrate();
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

        var roles = new List<Role>
        {
            new()
            {
                Id = SuperAdminRoleId, Name = "SuperAdmin",
                RoleType = RoleType.SuperAdmin
            },
            new()
            {
                Id = AdminRoleId, Name = "Admin",
                RoleType = RoleType.Admin
            },
            new()
            {
                Id = AcademyRoleId, Name = "Academy",
                RoleType = RoleType.Academy
            },
            new()
            {
                Id = CoachRoleId, Name = "Coach",
                RoleType = RoleType.Coach
            },
            new()
            {
                Id = AthleteRoleId, Name = "Athlete",
                RoleType = RoleType.Athlete
            }
        };
        dbContext.Roles.AddRange(roles);

        var users = new List<User>
        {
            new()
            {
                Id = TestAdminUserId,
                Email = "admin@test.com",
                PhoneNumber = "+1000000001",
                PasswordHash = "test-hash",
                FullName = "Test Admin",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestAcademyUserId,
                Email = "academy@test.com",
                PhoneNumber = "+1000000002",
                PasswordHash = "test-hash",
                FullName = "Test Academy User",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestCoachUserId,
                Email = "coach@test.com",
                PhoneNumber = "+1000000003",
                PasswordHash = "test-hash",
                FullName = "Test Coach",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestAthleteUserId,
                Email = "athlete@test.com",
                PhoneNumber = "+1000000004",
                PasswordHash = "test-hash",
                FullName = "Test Athlete",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            }
        };
        dbContext.Users.AddRange(users);

        var userRoles = new List<UserRole>
        {
            new()
            {
                UserId = TestAdminUserId, RoleId = AdminRoleId,
                AssignedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = TestAcademyUserId, RoleId = AcademyRoleId,
                AssignedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = TestCoachUserId, RoleId = CoachRoleId,
                AssignedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = TestAthleteUserId, RoleId = AthleteRoleId,
                AssignedAt = DateTime.UtcNow
            }
        };
        dbContext.UserRoles.AddRange(userRoles);

        var sportCategory = new SportCategory
        {
            Id = TestSportCategoryId,
            Name = "Racquet Sports",
            Description = "Sports played with racquets"
        };
        dbContext.SportCategories.Add(sportCategory);

        var sport = new Sport
        {
            Id = TestSportId,
            Name = "Badminton",
            Code = "BDM",
            OlympicSport = true,
            SportCategoryId = TestSportCategoryId
        };
        dbContext.Sports.Add(sport);

        var academy = new Academy
        {
            Id = TestAcademyId,
            AcademyCode = "ACD001",
            Name = "Test Sports Academy",
            Email = "contact@testsportsacademy.com",
            Phone = "+1000000005",
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified
        };
        dbContext.Academies.Add(academy);

        var branch = new AcademyBranch
        {
            Id = TestAcademyBranchId,
            AcademyId = TestAcademyId,
            BranchName = "Main Branch",
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        };
        dbContext.AcademyBranches.Add(branch);

        var coach = new Coach
        {
            Id = TestCoachId,
            UserId = TestCoachUserId,
            CoachCode = "COA001",
            RegistrationDate = DateTime.UtcNow,
            CoachingLevel = CoachingLevel.Senior,
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified
        };
        dbContext.Coaches.Add(coach);

        var athlete = new Athlete
        {
            Id = TestAthleteId,
            UserId = TestAthleteUserId,
            AthleteCode = "ATH001",
            RegistrationDate = DateTime.UtcNow,
            CurrentLevel = AthleteLevel.Intermediate,
            Status = AthleteStatus.Active
        };
        dbContext.Athletes.Add(athlete);

        var facility = new Facility
        {
            Id = TestFacilityId,
            AcademyId = TestAcademyId,
            FacilityCode = "FAC001",
            FacilityName = "Main Court",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 4,
            IndoorOutdoor = IndoorOutdoor.Indoor,
            Status = FacilityStatus.Active
        };
        dbContext.Facilities.Add(facility);

        dbContext.SaveChanges();
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
