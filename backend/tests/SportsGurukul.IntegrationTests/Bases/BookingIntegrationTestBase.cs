using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Infrastructure;
using Xunit;

namespace SportsGurukul.IntegrationTests.Bases;

[Collection("Postgres")]
public abstract class BookingIntegrationTestBase : IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient AdminClient;
    protected readonly HttpClient CoachClient;
    protected readonly HttpClient AthleteClient;
    protected readonly HttpClient UnauthenticatedClient;
    protected SeedResult SeedData = new();

    protected Guid TestAcademyId;
    protected Guid TestBranchId;
    protected Guid TestFacilityId;
    protected Guid TestCoachEntityId;
    protected Guid TestAthleteEntityId;

    protected BookingIntegrationTestBase(PostgresFixture postgresFixture)
    {
        Factory = new TestWebApplicationFactory();
        Factory.SetConnectionString(postgresFixture.ConnectionString);

        AdminClient = Factory.CreateClient();
        CoachClient = Factory.CreateClient();
        AthleteClient = Factory.CreateClient();
        UnauthenticatedClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Factory.InitializeAsync();
        await Factory.ResetDatabaseAsync();
        await SeedDatabaseAsync();
        await SeedBookingEntitiesAsync();
        SetAuthHeaders();
    }

    public async Task DisposeAsync()
    {
        AdminClient.Dispose();
        CoachClient.Dispose();
        AthleteClient.Dispose();
        UnauthenticatedClient.Dispose();
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seeder = new DatabaseSeeder(dbContext, passwordHasher);
        SeedData = await seeder.SeedAsync();
    }

    private async Task SeedBookingEntitiesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var now = DateTime.UtcNow;

        var academy = new Academy
        {
            Id = Guid.NewGuid(),
            AcademyCode = $"ACAD-{now:yyyyMMdd}-BOOK",
            Name = "Booking Test Academy",
            Email = $"bookingacademy{now.Ticks}@test.com",
            Phone = "+919800000001",
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            Description = "Academy for booking integration tests",
            CreatedAt = now,
            UpdatedBy = SeedData.AdminUserId
        };
        dbContext.Academies.Add(academy);
        TestAcademyId = academy.Id;

        var branch = new AcademyBranch
        {
            Id = Guid.NewGuid(),
            AcademyId = TestAcademyId,
            BranchName = "Booking Test Branch",
            Address = "456 Test Avenue",
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India",
            PostalCode = "400001",
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.AcademyBranches.Add(branch);
        TestBranchId = branch.Id;

        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            AcademyId = TestAcademyId,
            BranchId = TestBranchId,
            FacilityCode = $"FAC-{now:yyyyMMdd}-BOOK",
            FacilityName = "Booking Test Court",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 20,
            IndoorOutdoor = IndoorOutdoor.Indoor,
            LightingAvailable = true,
            ParkingAvailable = true,
            ChangingRoomAvailable = true,
            WashroomAvailable = true,
            MedicalRoomAvailable = true,
            Status = FacilityStatus.Active,
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.Facilities.Add(facility);
        TestFacilityId = facility.Id;

        var coachUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Booking Test Coach",
            Email = $"bookingcoach{now.Ticks}@test.com",
            PhoneNumber = "+919800000002",
            PasswordHash = "hashed",
            Status = UserStatus.Active,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.Users.Add(coachUser);

        var coach = new Coach
        {
            Id = Guid.NewGuid(),
            UserId = coachUser.Id,
            CoachCode = $"COA-{now:yyyyMMdd}-BOOK",
            Biography = "Coach for booking tests",
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.Coaches.Add(coach);
        TestCoachEntityId = coach.Id;

        var coachRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Coach");
        if (coachRole is not null)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = coachUser.Id,
                RoleId = coachRole.Id,
                AssignedAt = now
            });
        }

        var athleteUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Booking Test Athlete",
            Email = $"bookingathlete{now.Ticks}@test.com",
            PhoneNumber = "+919800000003",
            PasswordHash = "hashed",
            Status = UserStatus.Active,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.Users.Add(athleteUser);

        var athlete = new Domain.Entities.Athlete
        {
            Id = Guid.NewGuid(),
            UserId = athleteUser.Id,
            AthleteCode = $"ATH-{now:yyyyMMdd}-BOOK",
            CurrentLevel = AthleteLevel.Intermediate,
            ExperienceYears = 3,
            Status = AthleteStatus.Active,
            RegistrationDate = now,
            CreatedAt = now,
            CreatedBy = SeedData.AdminUserId
        };
        dbContext.Athletes.Add(athlete);
        TestAthleteEntityId = athlete.Id;

        var athleteRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Athlete");
        if (athleteRole is not null)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = athleteUser.Id,
                RoleId = athleteRole.Id,
                AssignedAt = now
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private void SetAuthHeaders()
    {
        AdminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.AdminUserId, "admin@sportsgurukul.com", "Admin User", new[] { "System Admin" }));
        CoachClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.CoachUserId, "coach@sportsgurukul.com", "Coach User", new[] { "Coach" }));
        AthleteClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.AthleteUserId, "athlete@sportsgurukul.com", "Athlete User", new[] { "Athlete" }));
    }

    protected HttpClient CreateClientWithRole(string role, Guid userId, string email, string fullName)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(userId, email, fullName, new[] { role }));
        return client;
    }

    protected async Task<BookingDto?> CreateBookingViaApiAsync(
        HttpClient client,
        Guid? academyId = null,
        Guid? facilityId = null,
        Guid? coachId = null,
        Guid? athleteId = null,
        DateTime? bookingDate = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        string title = "Test Booking")
    {
        var date = bookingDate ?? DateTime.UtcNow.Date.AddDays(1);
        var start = startTime ?? new TimeSpan(9, 0, 0);
        var end = endTime ?? new TimeSpan(10, 30, 0);

        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = title,
            Description = "Integration test booking",
            AcademyId = academyId ?? TestAcademyId,
            FacilityId = facilityId ?? TestFacilityId,
            CoachId = coachId ?? TestCoachEntityId,
            AthleteId = athleteId ?? TestAthleteEntityId,
            BookingDate = date,
            StartTime = start,
            EndTime = end
        };

        var response = await client.PostAsJsonAsync("/api/v1/bookings", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
            return content?.Data;
        }
        return null;
    }

    protected async Task<Guid> CreateBookingDirectlyInDbAsync(
        string status = "Pending",
        string? title = null,
        DateTime? bookingDate = null)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var now = DateTime.UtcNow;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = $"BK-{now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            BookingType = BookingType.FacilityReservation,
            Status = Enum.Parse<BookingStatus>(status),
            Title = title ?? "Direct DB Booking",
            Description = "Created directly in database for testing",
            AcademyId = TestAcademyId,
            BranchId = TestBranchId,
            FacilityId = TestFacilityId,
            CoachId = TestCoachEntityId,
            AthleteId = TestAthleteEntityId,
            BookingDate = bookingDate ?? DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            BookingCreatorId = SeedData.AdminUserId,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();
        return booking.Id;
    }

    protected async Task<Booking?> GetBookingFromDbAsync(Guid bookingId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    protected async Task<int> GetBookingCountAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Bookings.CountAsync();
    }

    protected async Task<BookingWaitlist?> GetWaitlistEntryFromDbAsync(Guid waitlistId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.BookingWaitlists.FirstOrDefaultAsync(w => w.Id == waitlistId);
    }

    protected async Task<BookingConflict?> GetConflictFromDbAsync(Guid conflictId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Set<BookingConflict>().FirstOrDefaultAsync(c => c.Id == conflictId);
    }
}
