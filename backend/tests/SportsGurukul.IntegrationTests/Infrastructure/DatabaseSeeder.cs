using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;

namespace SportsGurukul.IntegrationTests.Infrastructure;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Application.Common.Interfaces.IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext dbContext, Application.Common.Interfaces.IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<SeedResult> SeedAsync()
    {
        if (await _dbContext.Roles.AnyAsync())
        {
            return new SeedResult
            {
                AdminUserId = (await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@sportsgurukul.com"))?.Id ?? Guid.Empty,
                CoachUserId = (await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "coach@sportsgurukul.com"))?.Id ?? Guid.Empty,
                AthleteUserId = (await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "athlete@sportsgurukul.com"))?.Id ?? Guid.Empty,
                AcademyAdminUserId = (await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "academy@sportsgurukul.com"))?.Id ?? Guid.Empty,
                AthleteId = (await _dbContext.Athletes.FirstOrDefaultAsync(a => a.AthleteCode.StartsWith("ATH")))?.Id ?? Guid.Empty,
                CricketSportId = (await _dbContext.Sports.FirstOrDefaultAsync(s => s.Name == "Cricket"))?.Id ?? Guid.Empty,
                FootballSportId = (await _dbContext.Sports.FirstOrDefaultAsync(s => s.Name == "Football"))?.Id ?? Guid.Empty,
                TennisSportId = (await _dbContext.Sports.FirstOrDefaultAsync(s => s.Name == "Tennis"))?.Id ?? Guid.Empty,
                SportCategoryId = (await _dbContext.SportCategories.FirstOrDefaultAsync(c => c.Name == "Team Sports"))?.Id ?? Guid.Empty
            };
        }

        var sportCategory = new SportCategory
        {
            Id = Guid.NewGuid(),
            Name = "Team Sports",
            Description = "Team sports category"
        };
        _dbContext.SportCategories.Add(sportCategory);

        var individualCategory = new SportCategory
        {
            Id = Guid.NewGuid(),
            Name = "Individual Sports",
            Description = "Individual sports category"
        };
        _dbContext.SportCategories.Add(individualCategory);

        var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin", RoleType = RoleType.Admin, RolePermissions = new List<RolePermission>() };
        var coachRole = new Role { Id = Guid.NewGuid(), Name = "Coach", RoleType = RoleType.Coach, RolePermissions = new List<RolePermission>() };
        var athleteRole = new Role { Id = Guid.NewGuid(), Name = "Athlete", RoleType = RoleType.Athlete, RolePermissions = new List<RolePermission>() };
        var academyAdminRole = new Role { Id = Guid.NewGuid(), Name = "Academy Admin", RoleType = RoleType.Academy, RolePermissions = new List<RolePermission>() };

        _dbContext.Roles.AddRange(adminRole, coachRole, athleteRole, academyAdminRole);

        var adminUser = CreateUser("Admin User", "admin@sportsgurukul.com", "+919000000001");
        var coachUser = CreateUser("Coach User", "coach@sportsgurukul.com", "+919000000002");
        var athleteUser = CreateUser("Athlete User", "athlete@sportsgurukul.com", "+919000000003");
        var academyAdminUser = CreateUser("Academy Admin User", "academy@sportsgurukul.com", "+919000000004");

        _dbContext.Users.AddRange(adminUser, coachUser, athleteUser, academyAdminUser);

        _dbContext.UserRoles.AddRange(
            new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id, AssignedAt = DateTime.UtcNow },
            new UserRole { UserId = coachUser.Id, RoleId = coachRole.Id, AssignedAt = DateTime.UtcNow },
            new UserRole { UserId = athleteUser.Id, RoleId = athleteRole.Id, AssignedAt = DateTime.UtcNow },
            new UserRole { UserId = academyAdminUser.Id, RoleId = academyAdminRole.Id, AssignedAt = DateTime.UtcNow });

        var cricket = new Sport
        {
            Id = Guid.NewGuid(),
            Name = "Cricket",
            Code = "CRK",
            SportCategoryId = sportCategory.Id,
            Description = "Cricket sport"
        };
        var football = new Sport
        {
            Id = Guid.NewGuid(),
            Name = "Football",
            Code = "FBL",
            SportCategoryId = sportCategory.Id,
            Description = "Football sport"
        };
        var tennis = new Sport
        {
            Id = Guid.NewGuid(),
            Name = "Tennis",
            Code = "TNS",
            SportCategoryId = individualCategory.Id,
            Description = "Tennis sport"
        };

        _dbContext.Sports.AddRange(cricket, football, tennis);

        var athlete = new Athlete
        {
            Id = Guid.NewGuid(),
            UserId = athleteUser.Id,
            AthleteCode = $"ATH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CurrentLevel = AthleteLevel.Intermediate,
            ExperienceYears = 5,
            Height = "5'10\"",
            Weight = "75kg",
            BloodGroup = BloodGroup.OPositive,
            DominantHand = DominantHand.Right,
            DominantFoot = DominantFoot.Right,
            Biography = "Passionate athlete",
            Status = AthleteStatus.Active,
            RegistrationDate = DateTime.UtcNow
        };

        _dbContext.Athletes.Add(athlete);
        await _dbContext.SaveChangesAsync();

        return new SeedResult
        {
            AdminUserId = adminUser.Id,
            CoachUserId = coachUser.Id,
            AthleteUserId = athleteUser.Id,
            AcademyAdminUserId = academyAdminUser.Id,
            AthleteId = athlete.Id,
            CricketSportId = cricket.Id,
            FootballSportId = football.Id,
            TennisSportId = tennis.Id,
            SportCategoryId = sportCategory.Id
        };
    }

    private User CreateUser(string fullName, string email, string phone)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            PhoneNumber = phone,
            PasswordHash = _passwordHasher.HashPassword("Test@1234"),
            Status = UserStatus.Active,
            IsEmailVerified = true
        };
    }
}

public class SeedResult
{
    public Guid AdminUserId { get; set; }
    public Guid CoachUserId { get; set; }
    public Guid AthleteUserId { get; set; }
    public Guid AcademyAdminUserId { get; set; }
    public Guid AthleteId { get; set; }
    public Guid CricketSportId { get; set; }
    public Guid FootballSportId { get; set; }
    public Guid TennisSportId { get; set; }
    public Guid SportCategoryId { get; set; }
}
