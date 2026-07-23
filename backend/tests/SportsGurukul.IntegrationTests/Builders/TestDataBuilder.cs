using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.IntegrationTests.Builders;

public static class TestDataBuilder
{
    public static User CreateUser(
        string? fullName = null,
        string? email = null,
        string? phone = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName ?? $"Test User {Guid.NewGuid().ToString()[..6]}",
            Email = email ?? $"test{Guid.NewGuid().ToString()[..8]}@example.com",
            PhoneNumber = phone ?? $"+919{Random.Shared.Next(100000000, 999999999)}",
            PasswordHash = "hashed_password",
            Status = UserStatus.Active,
            IsEmailVerified = true
        };
    }

    public static Athlete CreateAthlete(
        Guid? userId = null,
        string? athleteCode = null,
        AthleteLevel level = AthleteLevel.Intermediate,
        int experience = 3)
    {
        return new Athlete
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            AthleteCode = athleteCode ?? $"ATH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CurrentLevel = level,
            ExperienceYears = experience,
            Height = "5'10\"",
            Weight = "75kg",
            BloodGroup = BloodGroup.OPositive,
            DominantHand = DominantHand.Right,
            DominantFoot = DominantFoot.Right,
            Biography = "Test biography",
            Status = AthleteStatus.Active,
            RegistrationDate = DateTime.UtcNow
        };
    }

    public static Sport CreateSport(string? name = null, Guid? sportCategoryId = null)
    {
        return new Sport
        {
            Id = Guid.NewGuid(),
            Name = name ?? $"Sport {Guid.NewGuid().ToString()[..6]}",
            Code = $"SP{Random.Shared.Next(100, 999)}",
            SportCategoryId = sportCategoryId ?? Guid.NewGuid(),
            Description = "Test sport"
        };
    }

    public static SportCategory CreateSportCategory(string? name = null)
    {
        return new SportCategory
        {
            Id = Guid.NewGuid(),
            Name = name ?? $"Category {Guid.NewGuid().ToString()[..6]}",
            Description = "Test category"
        };
    }

    public static Achievement CreateAchievement(string? title = null)
    {
        return new Achievement
        {
            Id = Guid.NewGuid(),
            Title = title ?? $"Achievement {Guid.NewGuid().ToString()[..6]}",
            Competition = "Test Competition",
            Position = "1st",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-30)
        };
    }

    public static MedicalProfile CreateMedicalProfile(Guid? athleteId = null)
    {
        return new MedicalProfile
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId ?? Guid.NewGuid(),
            BloodGroup = "O+",
            MedicalConditions = "None",
            Allergies = "None",
            Medications = "None",
            InsuranceNumber = "INS-001",
            DoctorName = "Dr. Test",
            DoctorContact = "+919000000000"
        };
    }

    public static EmergencyContact CreateEmergencyContact(Guid? athleteId = null)
    {
        return new EmergencyContact
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId ?? Guid.NewGuid(),
            Name = "Emergency Contact",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+919000000000",
            Email = "emergency@example.com"
        };
    }

    public static Ranking CreateRanking(Guid? athleteId = null)
    {
        return new Ranking
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId ?? Guid.NewGuid(),
            CurrentRank = "5",
            StateRank = "3",
            NationalRank = "100",
            InternationalRank = "500",
            RankingAuthority = "Test Authority"
        };
    }

    public static SavedSearch CreateSavedSearch(Guid? userId = null, string? name = null)
    {
        return new SavedSearch
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Name = name ?? $"Search {Guid.NewGuid().ToString()[..6]}",
            FiltersJson = "{}",
            UsageCount = 0
        };
    }

    public static RecentSearch CreateRecentSearch(Guid? userId = null, string? queryText = null)
    {
        return new RecentSearch
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            QueryText = queryText ?? $"query {Guid.NewGuid().ToString()[..6]}",
            FiltersJson = "{}",
            ResultCount = 5,
            SearchedAt = DateTime.UtcNow
        };
    }
}
