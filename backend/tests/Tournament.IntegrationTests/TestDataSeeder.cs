using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;

namespace Tournament.IntegrationTests;

public static class TestIds
{
    public static readonly Guid SportCategoryId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567801");
    public static readonly Guid SportId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567802");
    public static readonly Guid AcademyId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567803");
    public static readonly Guid AcademyBranchId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567804");
    public static readonly Guid FacilityId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567805");

    public static readonly Guid SystemAdminRoleId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567810");
    public static readonly Guid AcademyAdminRoleId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567811");
    public static readonly Guid CoachRoleId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567812");
    public static readonly Guid AthleteRoleId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567813");

    public static readonly Guid SystemAdminUserId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567820");
    public static readonly Guid AcademyAdminUserId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567821");
    public static readonly Guid CoachUserId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567822");
    public static readonly Guid AthleteUserId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567823");

    public static readonly Guid CoachEntityId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567830");
    public static readonly Guid AthleteEntityId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567831");
}

public static class TestConstants
{
    public const string SportCategoryName = "Team Sports";
    public const string SportName = "Cricket";
    public const string SportCode = "CRK";
    public const string AcademyName = "Test Sports Academy";
    public const string AcademyCode = "TSA-001";
    public const string AcademyEmail = "admin@testsportsacademy.com";
    public const string AcademyPhone = "+919100000001";
    public const string AcademyBranchName = "Main Branch";
    public const string FacilityName = "Main Indoor Stadium";
    public const string FacilityCode = "FAC-001";

    public const string SystemAdminEmail = "systemadmin@sportsgurukul.com";
    public const string SystemAdminName = "System Admin User";
    public const string AcademyAdminEmail = "academyadmin@sportsgurukul.com";
    public const string AcademyAdminName = "Academy Admin User";
    public const string CoachEmail = "coach@sportsgurukul.com";
    public const string CoachName = "Test Coach User";
    public const string AthleteEmail = "athlete@sportsgurukul.com";
    public const string AthleteName = "Test Athlete User";

    public const string SystemAdminRoleName = "System Admin";
    public const string AcademyAdminRoleName = "Academy Admin";
    public const string CoachRoleName = "Coach";
    public const string AthleteRoleName = "Athlete";

    public const string CoachCode = "COACH-001";
    public const string AthleteCode = "ATH-001";
}

public class TestDataSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public TestDataSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SeedData> SeedReferenceDataAsync()
    {
        var now = DateTime.UtcNow;

        var sportCategory = new SportCategory
        {
            Id = TestIds.SportCategoryId,
            Name = TestConstants.SportCategoryName,
            Description = "Test team sports category",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.SportCategories.Add(sportCategory);

        var sport = new Sport
        {
            Id = TestIds.SportId,
            Name = TestConstants.SportName,
            Code = TestConstants.SportCode,
            SportCategoryId = TestIds.SportCategoryId,
            OlympicSport = true,
            Description = "Test cricket sport",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Sports.Add(sport);

        var academy = new Academy
        {
            Id = TestIds.AcademyId,
            AcademyCode = TestConstants.AcademyCode,
            Name = TestConstants.AcademyName,
            Email = TestConstants.AcademyEmail,
            Phone = TestConstants.AcademyPhone,
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            Description = "Test academy for integration tests",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Academies.Add(academy);

        var academyBranch = new AcademyBranch
        {
            Id = TestIds.AcademyBranchId,
            AcademyId = TestIds.AcademyId,
            BranchName = TestConstants.AcademyBranchName,
            Address = "123 Test Street",
            City = "Bangalore",
            State = "Karnataka",
            Country = "India",
            PostalCode = "560001",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.AcademyBranches.Add(academyBranch);

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
            LightingAvailable = true,
            ParkingAvailable = true,
            ChangingRoomAvailable = true,
            WashroomAvailable = true,
            MedicalRoomAvailable = true,
            Status = FacilityStatus.Active,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Facilities.Add(facility);

        var systemAdminRole = new Role
        {
            Id = TestIds.SystemAdminRoleId,
            Name = TestConstants.SystemAdminRoleName,
            RoleType = RoleType.SuperAdmin,
            Description = "System administrator with full access",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var academyAdminRole = new Role
        {
            Id = TestIds.AcademyAdminRoleId,
            Name = TestConstants.AcademyAdminRoleName,
            RoleType = RoleType.Academy,
            Description = "Academy administrator",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var coachRole = new Role
        {
            Id = TestIds.CoachRoleId,
            Name = TestConstants.CoachRoleName,
            RoleType = RoleType.Coach,
            Description = "Coach role",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var athleteRole = new Role
        {
            Id = TestIds.AthleteRoleId,
            Name = TestConstants.AthleteRoleName,
            RoleType = RoleType.Athlete,
            Description = "Athlete role",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Roles.AddRange(systemAdminRole, academyAdminRole, coachRole, athleteRole);

        var systemAdminUser = new User
        {
            Id = TestIds.SystemAdminUserId,
            FullName = TestConstants.SystemAdminName,
            Email = TestConstants.SystemAdminEmail,
            PhoneNumber = "+919200000001",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var academyAdminUser = new User
        {
            Id = TestIds.AcademyAdminUserId,
            FullName = TestConstants.AcademyAdminName,
            Email = TestConstants.AcademyAdminEmail,
            PhoneNumber = "+919200000002",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var coachUser = new User
        {
            Id = TestIds.CoachUserId,
            FullName = TestConstants.CoachName,
            Email = TestConstants.CoachEmail,
            PhoneNumber = "+919200000003",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };

        var athleteUser = new User
        {
            Id = TestIds.AthleteUserId,
            FullName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            PhoneNumber = "+919200000004",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Users.AddRange(systemAdminUser, academyAdminUser, coachUser, athleteUser);

        _dbContext.UserRoles.AddRange(
            new UserRole { UserId = TestIds.SystemAdminUserId, RoleId = TestIds.SystemAdminRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.AcademyAdminUserId, RoleId = TestIds.AcademyAdminRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.CoachUserId, RoleId = TestIds.CoachRoleId, AssignedAt = now },
            new UserRole { UserId = TestIds.AthleteUserId, RoleId = TestIds.AthleteRoleId, AssignedAt = now }
        );

        var coach = new Coach
        {
            Id = TestIds.CoachEntityId,
            UserId = TestIds.CoachUserId,
            CoachCode = TestConstants.CoachCode,
            RegistrationDate = now,
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            YearsOfExperience = 5,
            Biography = "Test coach biography",
            CurrentOrganization = "Test Sports Academy",
            HighestQualification = "BSc Sports Science",
            PreferredLanguage = "English",
            CoachingLevel = CoachingLevel.Senior,
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Coaches.Add(coach);

        var athlete = new Athlete
        {
            Id = TestIds.AthleteEntityId,
            UserId = TestIds.AthleteUserId,
            AthleteCode = TestConstants.AthleteCode,
            RegistrationDate = now,
            Status = AthleteStatus.Active,
            CurrentLevel = AthleteLevel.Intermediate,
            ExperienceYears = 3,
            Height = "5'10\"",
            Weight = "75kg",
            BloodGroup = BloodGroup.OPositive,
            DominantHand = DominantHand.Right,
            DominantFoot = DominantFoot.Right,
            Biography = "Test athlete biography",
            CreatedAt = now,
            CreatedBy = TestIds.SystemAdminUserId
        };
        _dbContext.Athletes.Add(athlete);

        await _dbContext.SaveChangesAsync();

        return new SeedData
        {
            SportCategoryId = TestIds.SportCategoryId,
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            AcademyBranchId = TestIds.AcademyBranchId,
            FacilityId = TestIds.FacilityId,
            SystemAdminRoleId = TestIds.SystemAdminRoleId,
            AcademyAdminRoleId = TestIds.AcademyAdminRoleId,
            CoachRoleId = TestIds.CoachRoleId,
            AthleteRoleId = TestIds.AthleteRoleId,
            SystemAdminUserId = TestIds.SystemAdminUserId,
            AcademyAdminUserId = TestIds.AcademyAdminUserId,
            CoachUserId = TestIds.CoachUserId,
            AthleteUserId = TestIds.AthleteUserId,
            CoachEntityId = TestIds.CoachEntityId,
            AthleteEntityId = TestIds.AthleteEntityId
        };
    }
}

public record SeedData
{
    public Guid SportCategoryId { get; init; }
    public Guid SportId { get; init; }
    public Guid AcademyId { get; init; }
    public Guid AcademyBranchId { get; init; }
    public Guid FacilityId { get; init; }
    public Guid SystemAdminRoleId { get; init; }
    public Guid AcademyAdminRoleId { get; init; }
    public Guid CoachRoleId { get; init; }
    public Guid AthleteRoleId { get; init; }
    public Guid SystemAdminUserId { get; init; }
    public Guid AcademyAdminUserId { get; init; }
    public Guid CoachUserId { get; init; }
    public Guid AthleteUserId { get; init; }
    public Guid CoachEntityId { get; init; }
    public Guid AthleteEntityId { get; init; }
}