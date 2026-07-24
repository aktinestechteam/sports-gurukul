using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Common;

public static class TestDataBuilder
{
    public static User CreateUser(Guid? id = null, string? fullName = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FullName = fullName ?? "Test User",
        Email = "test@example.com",
        PhoneNumber = "+1234567890",
        Status = UserStatus.Active,
        UserRoles = new List<UserRole>
        {
            new() { Role = new Role { Id = Guid.NewGuid(), Name = "Athlete" } }
        }
    };

    public static Athlete CreateAthlete(Guid? id = null, Guid? userId = null, bool isDeleted = false) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        AthleteCode = $"ATH-{DateTime.UtcNow:yyyyMMdd}-TEST01",
        RegistrationDate = DateTime.UtcNow,
        CurrentLevel = AthleteLevel.Intermediate,
        ExperienceYears = 5,
        Height = "175cm",
        Weight = "70kg",
        BloodGroup = BloodGroup.OPositive,
        DominantHand = DominantHand.Right,
        DominantFoot = DominantFoot.Right,
        Biography = "Test biography",
        Status = AthleteStatus.Active,
        IsDeleted = isDeleted,
        User = CreateUser(),
        MedicalProfile = null,
        EmergencyContact = null,
        Ranking = null,
        AthleteSports = new List<AthleteSport>(),
        AthleteAchievements = new List<AthleteAchievement>()
    };

    public static Athlete CreateAthleteWithDetails(Guid? id = null, Guid? userId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        AthleteCode = $"ATH-{DateTime.UtcNow:yyyyMMdd}-DETAIL",
        RegistrationDate = DateTime.UtcNow,
        CurrentLevel = AthleteLevel.Advanced,
        ExperienceYears = 8,
        Height = "180cm",
        Weight = "75kg",
        BloodGroup = BloodGroup.APositive,
        DominantHand = DominantHand.Left,
        DominantFoot = DominantFoot.Ambidextrous,
        Biography = "Detailed athlete",
        Status = AthleteStatus.Active,
        User = CreateUser(),
        MedicalProfile = new MedicalProfile
        {
            Id = Guid.NewGuid(),
            MedicalConditions = "None",
            Allergies = "None",
            Medications = "None",
            BloodGroup = "A+",
            InsuranceNumber = "INS-001",
            DoctorName = "Dr. Smith",
            DoctorContact = "+1234567890"
        },
        EmergencyContact = new EmergencyContact
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1987654321",
            Email = "john@example.com"
        },
        Ranking = new Ranking
        {
            Id = Guid.NewGuid(),
            CurrentRank = "10",
            StateRank = "5",
            NationalRank = "50",
            InternationalRank = "500",
            RankingAuthority = "World Athletics"
        },
        AthleteSports = new List<AthleteSport>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                IsPrimarySport = true,
                JoinedDate = DateTime.UtcNow.AddYears(-3),
                Sport = new Sport
                {
                    Id = Guid.NewGuid(),
                    Name = "Cricket",
                    Code = "CRK",
                    OlympicSport = false,
                    SportCategory = new SportCategory { Id = Guid.NewGuid(), Name = "Team Sports" }
                }
            }
        },
        AthleteAchievements = new List<AthleteAchievement>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AchievementId = Guid.NewGuid(),
                AwardedDate = DateTime.UtcNow.AddDays(-30),
                Notes = "First achievement",
                Achievement = new Achievement
                {
                    Id = Guid.NewGuid(),
                    Title = "State Championship",
                    Competition = "State Level Cricket",
                    Position = "1st",
                    Level = AchievementLevel.State,
                    Date = DateTime.UtcNow.AddDays(-60)
                }
            }
        }
    };

    public static Sport CreateSport(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Cricket",
        Code = "CRK",
        OlympicSport = false,
        SportCategory = new SportCategory { Id = Guid.NewGuid(), Name = "Team Sports" }
    };

    public static Achievement CreateAchievement(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Title = "State Championship",
        Competition = "State Level Cricket",
        Position = "1st",
        Level = AchievementLevel.State,
        Date = DateTime.UtcNow.AddDays(-60)
    };

    public static AthleteSport CreateAthleteSport(Guid? athleteId = null, Guid? sportId = null) => new()
    {
        Id = Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        SportId = sportId ?? Guid.NewGuid(),
        IsPrimarySport = false,
        JoinedDate = DateTime.UtcNow,
        Sport = CreateSport(sportId)
    };

    public static Coach CreateCoach(Guid? id = null, Guid? userId = null, bool isDeleted = false) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        CoachCode = $"COACH-{DateTime.UtcNow:yyyyMMdd}-TEST01",
        RegistrationDate = DateTime.UtcNow,
        Biography = "Test coach biography",
        YearsOfExperience = 5,
        CurrentOrganization = "Test Academy",
        HighestQualification = "Masters in Sports",
        PreferredLanguage = "English",
        CoachingLevel = CoachingLevel.Senior,
        Status = CoachStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        IsDeleted = isDeleted,
        User = CreateUser(),
        CoachSports = new List<CoachSport>(),
        Certifications = new List<CoachCertification>(),
        Experiences = new List<CoachExperience>(),
        Education = new List<CoachEducation>()
    };

    public static Coach CreateCoachWithDetails(Guid? id = null, Guid? userId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        CoachCode = $"COACH-{DateTime.UtcNow:yyyyMMdd}-DTL01",
        RegistrationDate = DateTime.UtcNow,
        Biography = "Detailed coach biography",
        YearsOfExperience = 10,
        CurrentOrganization = "Elite Academy",
        HighestQualification = "PhD in Sports Science",
        PreferredLanguage = "English",
        CoachingLevel = CoachingLevel.Elite,
        Status = CoachStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        User = CreateUser(),
        Availability = new CoachAvailability
        {
            Id = Guid.NewGuid(),
            WeeklySchedule = "{}",
            TimeSlots = "[]",
            OnlineAvailable = true,
            OfflineAvailable = true,
            TravelDistance = 25
        },
        Location = new CoachLocation
        {
            Id = Guid.NewGuid(),
            Country = "India",
            State = "Maharashtra",
            City = "Mumbai",
            District = "Mumbai City",
            Latitude = 19.076m,
            Longitude = 72.877m
        },
        CoachSports = new List<CoachSport>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                IsPrimarySport = true,
                JoinedDate = DateTime.UtcNow.AddYears(-3),
                Sport = CreateSport()
            }
        },
        Certifications = new List<CoachCertification>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CertificationName = "BCCI Level A",
                IssuingAuthority = "BCCI",
                CertificateNumber = "BCCI-001",
                IssueDate = DateTime.UtcNow.AddYears(-2),
                ExpiryDate = DateTime.UtcNow.AddYears(3),
                VerificationStatus = VerificationStatus.Verified
            }
        },
        Experiences = new List<CoachExperience>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Organization = "State Cricket Academy",
                Role = "Head Coach",
                Sport = "Cricket",
                StartDate = DateTime.UtcNow.AddYears(-5),
                EndDate = DateTime.UtcNow.AddYears(-1)
            }
        },
        Education = new List<CoachEducation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Degree = "BPEd",
                Institution = "NIS",
                FieldOfStudy = "Sports Coaching",
                YearCompleted = 2018
            }
        }
    };

    public static CoachSport CreateCoachSport(Guid? coachId = null, Guid? sportId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        SportId = sportId ?? Guid.NewGuid(),
        IsPrimarySport = false,
        JoinedDate = DateTime.UtcNow,
        Sport = CreateSport(sportId)
    };

    public static CoachCertification CreateCoachCertification(Guid? coachId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        CertificationName = "Test Certification",
        IssuingAuthority = "Test Authority",
        VerificationStatus = VerificationStatus.Pending
    };

    public static CoachExperience CreateCoachExperience(Guid? coachId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        Organization = "Test Org",
        Role = "Coach",
        StartDate = DateTime.UtcNow.AddYears(-3)
    };

    public static CoachEducation CreateCoachEducation(Guid? coachId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        Degree = "BPEd",
        Institution = "NIS"
    };

    public static CoachAvailability CreateCoachAvailability(Guid? coachId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        WeeklySchedule = "{}",
        TimeSlots = "[]",
        OnlineAvailable = true,
        OfflineAvailable = true,
        TravelDistance = 25
    };

    public static CoachLocation CreateCoachLocation(Guid? coachId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        Latitude = 19.076m,
        Longitude = 72.877m
    };

    public static CoachAthlete CreateCoachAthlete(Guid? coachId = null, Guid? athleteId = null) => new()
    {
        Id = Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        AssignedDate = DateTime.UtcNow,
        IsActive = true,
        Athlete = CreateAthlete(id: athleteId)
    };
}
