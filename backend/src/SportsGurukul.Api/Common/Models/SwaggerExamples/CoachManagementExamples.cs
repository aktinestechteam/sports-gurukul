using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger response example for <see cref="MessageResponse"/>.
/// </summary>
public class MessageResponseExample : IExamplesProvider<MessageResponse>
{
    public MessageResponse GetExamples() => new()
    {
        Message = "Operation completed successfully."
    };
}

#region Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateCoachRequest"/>.
/// </summary>
public class CreateCoachRequestExample : IExamplesProvider<CreateCoachRequest>
{
    public CreateCoachRequest GetExamples() => new()
    {
        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        Biography = "Senior cricket coach with 10 years of experience coaching at state level.",
        YearsOfExperience = 10,
        CurrentOrganization = "Mumbai Cricket Academy",
        HighestQualification = "BCCI Level A Coaching Certificate",
        PreferredLanguage = "English",
        CoachingLevel = CoachingLevel.Senior
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateCoachProfileRequest"/>.
/// </summary>
public class UpdateCoachProfileRequestExample : IExamplesProvider<UpdateCoachProfileRequest>
{
    public UpdateCoachProfileRequest GetExamples() => new()
    {
        Biography = "Senior cricket coach with 12 years of experience coaching at national level.",
        YearsOfExperience = 12,
        CurrentOrganization = "National Cricket Academy",
        CoachingLevel = CoachingLevel.Elite
    };
}

/// <summary>
/// Swagger request example for <see cref="CoachAssignSportRequest"/>.
/// </summary>
public class CoachAssignSportRequestExample : IExamplesProvider<CoachAssignSportRequest>
{
    public CoachAssignSportRequest GetExamples() => new()
    {
        SportId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        IsPrimarySport = true
    };
}

/// <summary>
/// Swagger request example for <see cref="AddCertificationRequest"/>.
/// </summary>
public class AddCertificationRequestExample : IExamplesProvider<AddCertificationRequest>
{
    public AddCertificationRequest GetExamples() => new()
    {
        CertificationName = "BCCI Level A Coaching Certificate",
        IssuingAuthority = "Board of Control for Cricket in India",
        CertificateNumber = "BCCI-LA-2024-001",
        IssueDate = new DateTime(2024, 1, 15),
        ExpiryDate = new DateTime(2027, 1, 15),
        CertificateUrl = "https://cdn.sportsgurukul.com/certificates/coach-001.pdf"
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateCertificationRequest"/>.
/// </summary>
public class UpdateCertificationRequestExample : IExamplesProvider<UpdateCertificationRequest>
{
    public UpdateCertificationRequest GetExamples() => new()
    {
        CertificationName = "BCCI Level B Coaching Certificate",
        IssuingAuthority = "Board of Control for Cricket in India",
        CertificateNumber = "BCCI-LB-2025-002",
        ExpiryDate = new DateTime(2028, 3, 1)
    };
}

/// <summary>
/// Swagger request example for <see cref="VerifyCertificationRequest"/>.
/// </summary>
public class VerifyCertificationRequestExample : IExamplesProvider<VerifyCertificationRequest>
{
    public VerifyCertificationRequest GetExamples() => new()
    {
        Status = VerificationStatus.Verified
    };
}

/// <summary>
/// Swagger request example for <see cref="AddExperienceRequest"/>.
/// </summary>
public class AddExperienceRequestExample : IExamplesProvider<AddExperienceRequest>
{
    public AddExperienceRequest GetExamples() => new()
    {
        Organization = "Mumbai Cricket Academy",
        Role = "Head Coach",
        Sport = "Cricket",
        StartDate = new DateTime(2020, 1, 15),
        EndDate = new DateTime(2024, 6, 30),
        Description = "Coached under-19 cricket team to state championship finals."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateExperienceRequest"/>.
/// </summary>
public class UpdateExperienceRequestExample : IExamplesProvider<UpdateExperienceRequest>
{
    public UpdateExperienceRequest GetExamples() => new()
    {
        Organization = "National Cricket Academy",
        Role = "Assistant Coach",
        Description = "Led national junior team training program."
    };
}

/// <summary>
/// Swagger request example for <see cref="AddEducationRequest"/>.
/// </summary>
public class AddEducationRequestExample : IExamplesProvider<AddEducationRequest>
{
    public AddEducationRequest GetExamples() => new()
    {
        Degree = "Bachelor of Physical Education",
        Institution = "National Institute of Sports",
        FieldOfStudy = "Sports Coaching",
        YearCompleted = 2018
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateEducationRequest"/>.
/// </summary>
public class UpdateEducationRequestExample : IExamplesProvider<UpdateEducationRequest>
{
    public UpdateEducationRequest GetExamples() => new()
    {
        Degree = "Master of Sports Science",
        Institution = "Loughborough University",
        YearCompleted = 2020
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateAvailabilityRequest"/>.
/// </summary>
public class UpdateAvailabilityRequestExample : IExamplesProvider<UpdateAvailabilityRequest>
{
    public UpdateAvailabilityRequest GetExamples() => new()
    {
        WeeklySchedule = "{\"Monday\":\"06:00-18:00\",\"Tuesday\":\"06:00-18:00\",\"Wednesday\":\"06:00-18:00\",\"Thursday\":\"06:00-18:00\",\"Friday\":\"06:00-18:00\",\"Saturday\":\"08:00-14:00\"}",
        TimeSlots = "[\"06:00-08:00\",\"08:00-10:00\",\"10:00-12:00\",\"14:00-16:00\",\"16:00-18:00\"]",
        OnlineAvailable = true,
        OfflineAvailable = true,
        TravelDistance = 25
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateLocationRequest"/>.
/// </summary>
public class UpdateLocationRequestExample : IExamplesProvider<UpdateLocationRequest>
{
    public UpdateLocationRequest GetExamples() => new()
    {
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        District = "Mumbai City",
        Latitude = 19.0760m,
        Longitude = 72.8777m
    };
}

#endregion

#region Response Examples

/// <summary>
/// Swagger response example for <see cref="CoachDto"/>.
/// </summary>
public class CoachDtoExample : IExamplesProvider<CoachDto>
{
    public CoachDto GetExamples() => new()
    {
        Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        CoachCode = "COACH-20250101-SEED01",
        FullName = "Rajesh Kumar",
        Email = "rajesh@example.com",
        PhoneNumber = "+919876543210",
        ProfileImageUrl = "https://cdn.sportsgurukul.com/photos/rajesh.jpg",
        RegistrationDate = new DateTime(2025, 1, 1, 10, 30, 0, DateTimeKind.Utc),
        Biography = "Senior cricket coach with 10 years of experience.",
        YearsOfExperience = 10,
        CurrentOrganization = "Mumbai Cricket Academy",
        HighestQualification = "BCCI Level A",
        PreferredLanguage = "English",
        CoachingLevel = "Senior",
        Status = "Active",
        VerificationStatus = "Verified",
        CreatedAt = new DateTime(2025, 1, 1, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="CoachSummaryDto"/>.
/// </summary>
public class CoachSummaryDtoExample : IExamplesProvider<CoachSummaryDto>
{
    public CoachSummaryDto GetExamples() => new()
    {
        Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        CoachCode = "COACH-20250101-SEED01",
        FullName = "Rajesh Kumar",
        Email = "rajesh@example.com",
        CoachingLevel = "Senior",
        Status = "Active",
        VerificationStatus = "Verified",
        PrimarySport = "Cricket",
        YearsOfExperience = 10,
        City = "Mumbai",
        State = "Maharashtra",
        IsVerified = true,
        IsOnlineAvailable = true,
        CreatedAt = new DateTime(2025, 1, 1, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="SportDto"/>.
/// </summary>
public class CoachSportDtoExample : IExamplesProvider<SportDto>
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
/// Swagger response example for <see cref="CertificationDto"/>.
/// </summary>
public class CertificationDtoExample : IExamplesProvider<CertificationDto>
{
    public CertificationDto GetExamples() => new()
    {
        Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
        CertificationName = "BCCI Level A Coaching Certificate",
        IssuingAuthority = "Board of Control for Cricket in India",
        CertificateNumber = "BCCI-LA-2024-001",
        IssueDate = new DateTime(2024, 1, 15),
        ExpiryDate = new DateTime(2027, 1, 15),
        VerificationStatus = "Verified",
        CertificateUrl = "https://cdn.sportsgurukul.com/certificates/coach-001.pdf",
        IsExpired = false,
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="ExperienceDto"/>.
/// </summary>
public class ExperienceDtoExample : IExamplesProvider<ExperienceDto>
{
    public ExperienceDto GetExamples() => new()
    {
        Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"),
        Organization = "State Cricket Academy",
        Role = "Head Coach",
        Sport = "Cricket",
        StartDate = new DateTime(2020, 1, 15),
        EndDate = new DateTime(2024, 6, 30),
        Description = "Coached under-19 cricket team to state championship finals.",
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="EducationDto"/>.
/// </summary>
public class EducationDtoExample : IExamplesProvider<EducationDto>
{
    public EducationDto GetExamples() => new()
    {
        Id = Guid.Parse("a2000000-0000-0000-0000-000000000001"),
        Degree = "Bachelor of Physical Education",
        Institution = "National Institute of Sports",
        FieldOfStudy = "Sports Coaching",
        YearCompleted = 2018,
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="AvailabilityDto"/>.
/// </summary>
public class AvailabilityDtoExample : IExamplesProvider<AvailabilityDto>
{
    public AvailabilityDto GetExamples() => new()
    {
        Id = Guid.Parse("b2000000-0000-0000-0000-000000000001"),
        WeeklySchedule = "{\"Monday\":\"06:00-18:00\",\"Tuesday\":\"06:00-18:00\",\"Wednesday\":\"06:00-18:00\",\"Thursday\":\"06:00-18:00\",\"Friday\":\"06:00-18:00\",\"Saturday\":\"08:00-14:00\"}",
        TimeSlots = "[\"06:00-08:00\",\"08:00-10:00\",\"10:00-12:00\",\"14:00-16:00\",\"16:00-18:00\"]",
        OnlineAvailable = true,
        OfflineAvailable = true,
        TravelDistance = 25,
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="LocationDto"/>.
/// </summary>
public class LocationDtoExample : IExamplesProvider<LocationDto>
{
    public LocationDto GetExamples() => new()
    {
        Id = Guid.Parse("c2000000-0000-0000-0000-000000000001"),
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        District = "Mumbai City",
        Latitude = 19.0760m,
        Longitude = 72.8777m,
        CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="AssignedAthleteDto"/>.
/// </summary>
public class AssignedAthleteDtoExample : IExamplesProvider<AssignedAthleteDto>
{
    public AssignedAthleteDto GetExamples() => new()
    {
        Id = Guid.NewGuid(),
        AthleteId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AthleteCode = "ATH-20250615-A1B2C3",
        FullName = "Rahul Sharma",
        Email = "rahul@example.com",
        PhoneNumber = "+919876543211",
        CurrentLevel = "Intermediate",
        Status = "Active",
        PrimarySport = "Cricket",
        AssignedDate = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc)
    };
}

/// <summary>
/// Swagger response example for <see cref="CoachSearchResponse"/>.
/// </summary>
public class CoachSearchResponseExample : IExamplesProvider<CoachSearchResponse>
{
    public CoachSearchResponse GetExamples() => new()
    {
        Items =
        [
            new CoachSummaryDto
            {
                Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                CoachCode = "COACH-20250101-SEED01",
                FullName = "Rajesh Kumar",
                Email = "rajesh@example.com",
                CoachingLevel = "Senior",
                Status = "Active",
                VerificationStatus = "Verified",
                PrimarySport = "Cricket",
                YearsOfExperience = 10,
                City = "Mumbai",
                State = "Maharashtra",
                IsVerified = true,
                IsOnlineAvailable = true,
                CreatedAt = new DateTime(2025, 1, 1, 10, 30, 0, DateTimeKind.Utc)
            },
            new CoachSummaryDto
            {
                Id = Guid.Parse("d1000000-0000-0000-0000-000000000002"),
                UserId = Guid.NewGuid(),
                CoachCode = "COACH-20250310-D4E5F6",
                FullName = "Priya Patel",
                Email = "priya@example.com",
                CoachingLevel = "Intermediate",
                Status = "Active",
                VerificationStatus = "Verified",
                PrimarySport = "Badminton",
                YearsOfExperience = 5,
                City = "Pune",
                State = "Maharashtra",
                IsVerified = true,
                IsOnlineAvailable = true,
                IsOfflineAvailable = true,
                CreatedAt = new DateTime(2025, 3, 10, 8, 0, 0, DateTimeKind.Utc)
            }
        ],
        TotalRecords = 42,
        TotalPages = 3,
        CurrentPage = 1,
        PageSize = 20
    };
}

#endregion
