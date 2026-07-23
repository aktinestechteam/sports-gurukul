using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger request example for <see cref="CreateAthleteRequest"/>.
/// </summary>
public class CreateAthleteRequestExample : IExamplesProvider<CreateAthleteRequest>
{
    public CreateAthleteRequest GetExamples() => new()
    {
        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        CurrentLevel = AthleteLevel.Intermediate,
        ExperienceYears = 5,
        Height = "5'10\"",
        Weight = "75kg",
        BloodGroup = BloodGroup.OPositive,
        DominantHand = DominantHand.Right,
        DominantFoot = DominantFoot.Right,
        Biography = "Passionate cricket player with 5 years of experience."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateAthleteRequest"/>.
/// </summary>
public class UpdateAthleteRequestExample : IExamplesProvider<UpdateAthleteRequest>
{
    public UpdateAthleteRequest GetExamples() => new()
    {
        CurrentLevel = AthleteLevel.Advanced,
        ExperienceYears = 7,
        Height = "6'0\"",
        Weight = "78kg",
        Biography = "National-level sprinter focused on 100m and 200m events."
    };
}

/// <summary>
/// Swagger request example for <see cref="AssignSportRequest"/>.
/// </summary>
public class AssignSportRequestExample : IExamplesProvider<AssignSportRequest>
{
    public AssignSportRequest GetExamples() => new()
    {
        SportId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        IsPrimarySport = true
    };
}

/// <summary>
/// Swagger request example for <see cref="AddAchievementRequest"/>.
/// </summary>
public class AddAchievementRequestExample : IExamplesProvider<AddAchievementRequest>
{
    public AddAchievementRequest GetExamples() => new()
    {
        Title = "State Championship Gold Medal",
        Competition = "Maharashtra State Athletics Championship 2025",
        Position = "1st Place",
        Level = AchievementLevel.State,
        Date = new DateTime(2025, 3, 15),
        CertificateUrl = "https://cdn.sportsgurukul.com/certificates/ath-001.pdf",
        Notes = "First place in the 100m sprint with a personal best time."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateAchievementRequest"/>.
/// </summary>
public class UpdateAchievementRequestExample : IExamplesProvider<UpdateAchievementRequest>
{
    public UpdateAchievementRequest GetExamples() => new()
    {
        Title = "National Championship Silver Medal",
        Competition = "National Athletics Championship 2025",
        Position = "2nd Place",
        Level = AchievementLevel.National,
        Notes = "Personal best time in the 200m event."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateMedicalProfileRequest"/>.
/// </summary>
public class UpdateMedicalProfileRequestExample : IExamplesProvider<UpdateMedicalProfileRequest>
{
    public UpdateMedicalProfileRequest GetExamples() => new()
    {
        MedicalConditions = "Asthma (mild)",
        Allergies = "Penicillin",
        Medications = "Inhaler as needed",
        BloodGroup = "O Positive",
        InsuranceNumber = "INS-2025-001234",
        DoctorName = "Dr. Anjali Mehta",
        DoctorContact = "+919876543210"
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateEmergencyContactRequest"/>.
/// </summary>
public class UpdateEmergencyContactRequestExample : IExamplesProvider<UpdateEmergencyContactRequest>
{
    public UpdateEmergencyContactRequest GetExamples() => new()
    {
        Name = "Sunita Sharma",
        Relationship = EmergencyRelationship.Parent,
        Phone = "+919876543210",
        Email = "sunita.sharma@example.com"
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateRankingRequest"/>.
/// </summary>
public class UpdateRankingRequestExample : IExamplesProvider<UpdateRankingRequest>
{
    public UpdateRankingRequest GetExamples() => new()
    {
        CurrentRank = "A+",
        StateRank = "12",
        NationalRank = "156",
        InternationalRank = "2500",
        RankingAuthority = "Athletics Federation of India"
    };
}

/// <summary>
/// Swagger response example for <see cref="AthleteDto"/>.
/// </summary>
public class AthleteDtoExample : IExamplesProvider<AthleteDto>
{
    public AthleteDto GetExamples() => new()
    {
        Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        UserId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
        AthleteCode = "ATH-20250615-A1B2C3",
        FullName = "Rahul Sharma",
        Email = "rahul@example.com",
        PhoneNumber = "+919876543210",
        ProfileImageUrl = "https://cdn.sportsgurukul.com/photos/rahul.jpg",
        RegistrationDate = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc),
        CurrentLevel = "Intermediate",
        ExperienceYears = 5,
        Height = "5'10\"",
        Weight = "75kg",
        Status = "Active",
        Roles = ["Athlete"],
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="SportDto"/>.
/// </summary>
public class SportDtoExample : IExamplesProvider<SportDto>
{
    public SportDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        SportId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        Name = "Cricket",
        Code = "CRK",
        CategoryName = "Bat & Ball",
        OlympicSport = false,
        IsPrimarySport = true,
        JoinedDate = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="AthleteAchievementDto"/>.
/// </summary>
public class AthleteAchievementDtoExample : IExamplesProvider<AthleteAchievementDto>
{
    public AthleteAchievementDto GetExamples() => new()
    {
        Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        AchievementId = Guid.Parse("d4e5f6a7-b8c9-0123-def0-234567890123"),
        Title = "State Championship Gold Medal",
        Competition = "Maharashtra State Athletics Championship 2025",
        Position = "1st Place",
        Level = "State",
        Date = new DateTime(2025, 3, 15),
        CertificateUrl = "https://cdn.sportsgurukul.com/certificates/ath-001.pdf",
        AwardedDate = new DateTime(2025, 3, 16, 10, 0, 0, DateTimeKind.Utc),
        Notes = "First place in the 100m sprint."
    };
}

/// <summary>
/// Swagger response example for <see cref="RankingDto"/>.
/// </summary>
public class RankingDtoExample : IExamplesProvider<RankingDto>
{
    public RankingDto GetExamples() => new()
    {
        Id = Guid.Parse("e5f6a7b8-c9d0-1234-ef01-345678901234"),
        CurrentRank = "A+",
        StateRank = "12",
        NationalRank = "156",
        InternationalRank = "2500",
        RankingAuthority = "Athletics Federation of India",
        CreatedAt = new DateTime(2025, 6, 1, 8, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 7, 1, 8, 0, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="AthleteSearchResponse"/>.
/// </summary>
public class AthleteSearchResponseExample : IExamplesProvider<AthleteSearchResponse>
{
    public AthleteSearchResponse GetExamples() => new()
    {
        Items =
        [
            new AthleteSummaryDto
            {
                Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                UserId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                AthleteCode = "ATH-20250615-A1B2C3",
                FullName = "Rahul Sharma",
                Email = "rahul@example.com",
                ProfileImageUrl = "https://cdn.sportsgurukul.com/photos/rahul.jpg",
                CurrentLevel = "Intermediate",
                Status = "Active",
                PrimarySport = "Cricket",
                CurrentRank = "A+",
                ExperienceYears = 5,
                CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
            },
            new AthleteSummaryDto
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                UserId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                AthleteCode = "ATH-20250310-D4E5F6",
                FullName = "Priya Patel",
                Email = "priya@example.com",
                CurrentLevel = "Advanced",
                Status = "Active",
                PrimarySport = "Badminton",
                ExperienceYears = 8,
                CreatedAt = new DateTime(2025, 3, 10, 8, 0, 0, DateTimeKind.Utc)
            }
        ],
        TotalRecords = 42,
        TotalPages = 3,
        CurrentPage = 1,
        PageSize = 20
    };
}
