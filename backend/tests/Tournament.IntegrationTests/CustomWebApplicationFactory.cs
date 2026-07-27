using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using SportsGurukul.Api;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Infrastructure.Caching;
using SportsGurukul.Infrastructure.Email;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories;
using SportsGurukul.Infrastructure.Storage;
using SportsGurukul.Platform.Competition;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Testcontainers.PostgreSql;

namespace Tournament.IntegrationTests;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    private static readonly PostgreSqlContainer Container;
    private static bool _databaseInitialized;
    private static readonly object DatabaseLock = new();

    public string ConnectionString => Container.GetConnectionString();

    public const string TestJwtSigningKey = "Integration-Test-Secret-Key-At-Least-32-Characters-Long!!";
    public const string TestJwtIssuer = "SportsGurukul";
    public const string TestJwtAudience = "SportsGurukul";

    static CustomWebApplicationFactory()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("sportsgurukul_tournament_test")
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

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString,
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Jwt:Issuer"] = TestJwtIssuer,
                ["Jwt:Audience"] = TestJwtAudience,
                ["Jwt:SigningKey"] = TestJwtSigningKey,
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"] = "30",
                ["Storage:Provider"] = "Local",
                ["Storage:BasePath"] = "test-uploads"
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoveService<DbContextOptions<ApplicationDbContext>>(services);
            RemoveService<ApplicationDbContext>(services);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddInfrastructureServicesForTest();
            services.AddCompetitionEngine();

            services.Configure<JwtOptions>(opts =>
            {
                opts.Issuer = TestJwtIssuer;
                opts.Audience = TestJwtAudience;
                opts.SigningKey = TestJwtSigningKey;
                opts.AccessTokenExpirationMinutes = 60;
            });

            services.Configure<StorageOptions>(opts =>
            {
                opts.Provider = StorageProvider.Local;
                opts.BasePath = Path.Combine(Path.GetTempPath(), "sportsgurukul_test_uploads");
            });

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                opts =>
                {
                    opts.RequireHttpsMetadata = false;
                    opts.SaveToken = true;
                    opts.TokenValidationParameters = new TokenValidationParameters
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

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
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

        var now = DateTime.UtcNow;

        var roles = new List<Role>
        {
            new()
            {
                Id = TestIds.SystemAdminRoleId, Name = TestConstants.SystemAdminRoleName,
                RoleType = RoleType.SuperAdmin
            },
            new()
            {
                Id = TestIds.AcademyAdminRoleId, Name = TestConstants.AcademyAdminRoleName,
                RoleType = RoleType.Academy
            },
            new()
            {
                Id = TestIds.CoachRoleId, Name = TestConstants.CoachRoleName,
                RoleType = RoleType.Coach
            },
            new()
            {
                Id = TestIds.AthleteRoleId, Name = TestConstants.AthleteRoleName,
                RoleType = RoleType.Athlete
            }
        };
        dbContext.Roles.AddRange(roles);

        var users = new List<User>
        {
            new()
            {
                Id = TestIds.SystemAdminUserId,
                Email = TestConstants.SystemAdminEmail,
                FullName = TestConstants.SystemAdminName,
                PhoneNumber = "+919200000001",
                PasswordHash = "test-hash",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestIds.AcademyAdminUserId,
                Email = TestConstants.AcademyAdminEmail,
                FullName = TestConstants.AcademyAdminName,
                PhoneNumber = "+919200000002",
                PasswordHash = "test-hash",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestIds.CoachUserId,
                Email = TestConstants.CoachEmail,
                FullName = TestConstants.CoachName,
                PhoneNumber = "+919200000003",
                PasswordHash = "test-hash",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            },
            new()
            {
                Id = TestIds.AthleteUserId,
                Email = TestConstants.AthleteEmail,
                FullName = TestConstants.AthleteName,
                PhoneNumber = "+919200000004",
                PasswordHash = "test-hash",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true
            }
        };
        dbContext.Users.AddRange(users);

        dbContext.UserRoles.AddRange(
            new UserRole { UserId = TestIds.SystemAdminUserId, RoleId = TestIds.SystemAdminRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.AcademyAdminUserId, RoleId = TestIds.AcademyAdminRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.CoachUserId, RoleId = TestIds.CoachRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.AthleteUserId, RoleId = TestIds.AthleteRoleId, AssignedAt = now }
        );

        var sportCategory = new SportCategory
        {
            Id = TestIds.SportCategoryId,
            Name = TestConstants.SportCategoryName,
            Description = "Test team sports category"
        };
        dbContext.SportCategories.Add(sportCategory);

        var sport = new Sport
        {
            Id = TestIds.SportId,
            Name = TestConstants.SportName,
            Code = TestConstants.SportCode,
            SportCategoryId = TestIds.SportCategoryId,
            OlympicSport = true,
            Description = "Test cricket sport"
        };
        dbContext.Sports.Add(sport);

        var academy = new Academy
        {
            Id = TestIds.AcademyId,
            AcademyCode = TestConstants.AcademyCode,
            Name = TestConstants.AcademyName,
            Email = TestConstants.AcademyEmail,
            Phone = TestConstants.AcademyPhone,
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            Description = "Test academy for integration tests"
        };
        dbContext.Academies.Add(academy);

        var branch = new AcademyBranch
        {
            Id = TestIds.AcademyBranchId,
            AcademyId = TestIds.AcademyId,
            BranchName = TestConstants.AcademyBranchName,
            City = "Bangalore",
            State = "Karnataka",
            Country = "India"
        };
        dbContext.AcademyBranches.Add(branch);

        var facility = new Facility
        {
            Id = TestIds.FacilityId,
            AcademyId = TestIds.AcademyId,
            BranchId = TestIds.AcademyBranchId,
            FacilityCode = TestConstants.FacilityCode,
            FacilityName = TestConstants.FacilityName,
            FacilityType = FacilityType.IndoorStadium,
            Capacity = 50,
            IndoorOutdoor = IndoorOutdoor.Indoor,
            Status = FacilityStatus.Active
        };
        dbContext.Facilities.Add(facility);

        var coach = new Coach
        {
            Id = TestIds.CoachEntityId,
            UserId = TestIds.CoachUserId,
            CoachCode = TestConstants.CoachCode,
            RegistrationDate = now,
            CoachingLevel = CoachingLevel.Senior,
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified
        };
        dbContext.Coaches.Add(coach);

        var athlete = new Athlete
        {
            Id = TestIds.AthleteEntityId,
            UserId = TestIds.AthleteUserId,
            AthleteCode = TestConstants.AthleteCode,
            RegistrationDate = now,
            CurrentLevel = AthleteLevel.Intermediate,
            Status = AthleteStatus.Active
        };
        dbContext.Athletes.Add(athlete);

        dbContext.SaveChanges();
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor is not null)
            services.Remove(descriptor);
    }

    public async Task InitializeAsync()
    {
    }

    public new async Task DisposeAsync()
    {
        await Container.DisposeAsync();
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
        services.AddScoped<IAcademyRepository, AcademyRepository>();
        services.AddScoped<IAcademyBranchRepository, AcademyBranchRepository>();
        services.AddScoped<IAcademyFacilityRepository, AcademyFacilityRepository>();
        services.AddScoped<IAcademyMembershipRepository, AcademyMembershipRepository>();
        services.AddScoped<ICoachAcademyRepository, CoachAcademyRepository>();
        services.AddScoped<IAthleteAcademyRepository, AthleteAcademyRepository>();
        services.AddScoped<IAcademySearchRepository, AcademySearchRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFacilityCourtRepository, FacilityCourtRepository>();
        services.AddScoped<IFacilityEquipmentRepository, FacilityEquipmentRepository>();
        services.AddScoped<IFacilityScheduleRepository, FacilityScheduleRepository>();
        services.AddScoped<IFacilityPricingRepository, FacilityPricingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingScheduleRepository, BookingScheduleRepository>();
        services.AddScoped<IConflictRepository, ConflictRepository>();
        services.AddScoped<IWaitlistRepository, WaitlistRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IBracketRepository, BracketRepository>();
        services.AddScoped<IRankingRepository, RankingRepository>();
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