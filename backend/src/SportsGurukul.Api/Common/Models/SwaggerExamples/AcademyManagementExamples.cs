using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

#region Academy Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateAcademyRequest"/>.
/// </summary>
public class CreateAcademyRequestExample : IExamplesProvider<CreateAcademyRequest>
{
    public CreateAcademyRequest GetExamples() => new()
    {
        Name = "Mumbai Sports Academy",
        LegalName = "Mumbai Sports Academy Pvt. Ltd.",
        Description = "Premier multi-sport academy in Mumbai.",
        RegistrationNumber = "REG-2025-001234",
        GSTNumber = "27AABCU9603R1ZM",
        EstablishedDate = new DateTime(2020, 6, 15),
        Website = "https://mumbaisportsacademy.com",
        Email = "info@mumbaisportsacademy.com",
        Phone = "+919876543210",
        AcademyType = "MultiSport",
        PrimaryContactName = "Rajesh Kumar",
        Address = "123 Sports Avenue, Andheri West",
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        PostalCode = "400058",
        SportNames = ["Cricket", "Football"]
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateAcademyRequest"/>.
/// </summary>
public class UpdateAcademyRequestExample : IExamplesProvider<UpdateAcademyRequest>
{
    public UpdateAcademyRequest GetExamples() => new()
    {
        Name = "Mumbai Sports Academy 2.0",
        Description = "One of Mumbai's leading multi-sport academies.",
        Email = "contact@msa-new.com",
        Phone = "+919876543211",
        LogoUrl = "https://cdn.sportsgurukul.com/logos/msa.png",
        BannerUrl = "https://cdn.sportsgurukul.com/banners/msa.jpg"
    };
}

/// <summary>
/// Swagger request example for <see cref="VerifyAcademyRequest"/>.
/// </summary>
public class VerifyAcademyRequestExample : IExamplesProvider<VerifyAcademyRequest>
{
    public VerifyAcademyRequest GetExamples() => new()
    {
        Remarks = "Academy documentation verified successfully."
    };
}

/// <summary>
/// Swagger request example for <see cref="RejectAcademyVerificationRequest"/>.
/// </summary>
public class RejectAcademyVerificationRequestExample : IExamplesProvider<RejectAcademyVerificationRequest>
{
    public RejectAcademyVerificationRequest GetExamples() => new()
    {
        Remarks = "Registration document is expired. Please update and resubmit."
    };
}

#endregion

#region Branch Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateBranchRequest"/>.
/// </summary>
public class CreateBranchRequestExample : IExamplesProvider<CreateBranchRequest>
{
    public CreateBranchRequest GetExamples() => new()
    {
        BranchName = "Andheri Branch",
        Address = "123 Sports Avenue, Andheri West",
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        District = "Andheri",
        PostalCode = "400058",
        Latitude = 19.1364m,
        Longitude = 72.8296m
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateBranchRequest"/>.
/// </summary>
public class UpdateBranchRequestExample : IExamplesProvider<UpdateBranchRequest>
{
    public UpdateBranchRequest GetExamples() => new()
    {
        BranchName = "Andheri West Branch",
        Address = "456 Sports Lane, Andheri West",
        City = "Mumbai"
    };
}

#endregion

#region Facility Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateFacilityRequest"/>.
/// </summary>
public class CreateFacilityRequestExample : IExamplesProvider<CreateFacilityRequest>
{
    public CreateFacilityRequest GetExamples() => new()
    {
        FacilityName = "Main Cricket Ground",
        FacilityType = AcademyFacilityType.Field,
        IndoorOutdoor = "Outdoor",
        Capacity = 200,
        Available = true,
        Description = "Floodlit cricket ground with turf wicket."
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateFacilityRequest"/>.
/// </summary>
public class UpdateFacilityRequestExample : IExamplesProvider<UpdateFacilityRequest>
{
    public UpdateFacilityRequest GetExamples() => new()
    {
        FacilityName = "Cricket Ground - North",
        Capacity = 250,
        Available = true,
        Description = "Floodlit cricket ground with two turf wickets."
    };
}

#endregion

#region Membership Request Examples

/// <summary>
/// Swagger request example for <see cref="CreateMembershipPlanRequest"/>.
/// </summary>
public class CreateMembershipPlanRequestExample : IExamplesProvider<CreateMembershipPlanRequest>
{
    public CreateMembershipPlanRequest GetExamples() => new()
    {
        MembershipName = "Gold Monthly Plan",
        Description = "Unlimited access to all facilities for 30 days.",
        Price = 2500.00m,
        Duration = 30,
        Benefits = "Unlimited gym access, 2 coaching sessions, locker access"
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateMembershipPlanRequest"/>.
/// </summary>
public class UpdateMembershipPlanRequestExample : IExamplesProvider<UpdateMembershipPlanRequest>
{
    public UpdateMembershipPlanRequest GetExamples() => new()
    {
        MembershipName = "Gold Monthly Plan v2",
        Price = 3000.00m,
        Benefits = "Unlimited access, 4 coaching sessions, locker, physio"
    };
}

#endregion

#region Contact Request Examples

/// <summary>
/// Swagger request example for <see cref="UpdateAcademyContactRequest"/>.
/// </summary>
public class UpdateAcademyContactRequestExample : IExamplesProvider<UpdateAcademyContactRequest>
{
    public UpdateAcademyContactRequest GetExamples() => new()
    {
        PrimaryContactName = "Rajesh Kumar",
        PrimaryPhone = "+919876543210",
        PrimaryEmail = "rajesh@mumbaisportsacademy.com",
        SecondaryContactName = "Priya Sharma",
        SecondaryPhone = "+919876543211",
        SecondaryEmail = "priya@mumbaisportsacademy.com",
        Address = "123 Sports Avenue, Andheri West, Mumbai",
        Country = "India",
        State = "Maharashtra",
        City = "Mumbai",
        PostalCode = "400058",
        Latitude = 19.1364m,
        Longitude = 72.8296m
    };
}

#endregion

#region Operating Hours Request Examples

/// <summary>
/// Swagger request example for <see cref="UpdateOperatingHoursRequest"/>.
/// </summary>
public class UpdateOperatingHoursRequestExample : IExamplesProvider<UpdateOperatingHoursRequest>
{
    public UpdateOperatingHoursRequest GetExamples() => new()
    {
        MondayOpening = "06:00",
        MondayClosing = "21:00",
        TuesdayOpening = "06:00",
        TuesdayClosing = "21:00",
        WednesdayOpening = "06:00",
        WednesdayClosing = "21:00",
        ThursdayOpening = "06:00",
        ThursdayClosing = "21:00",
        FridayOpening = "06:00",
        FridayClosing = "21:00",
        SaturdayOpening = "07:00",
        SaturdayClosing = "19:00",
        SundayOpening = "07:00",
        SundayClosing = "14:00",
        HolidaySchedule = "Closed on national holidays."
    };
}

#endregion

#region Social Links Request Examples

/// <summary>
/// Swagger request example for <see cref="UpdateSocialLinksRequest"/>.
/// </summary>
public class UpdateSocialLinksRequestExample : IExamplesProvider<UpdateSocialLinksRequest>
{
    public UpdateSocialLinksRequest GetExamples() => new()
    {
        Links =
        [
            new SocialLinkInput { Platform = "Instagram", Url = "https://instagram.com/mumbaisportsacademy" },
            new SocialLinkInput { Platform = "Facebook", Url = "https://facebook.com/mumbaisportsacademy" },
            new SocialLinkInput { Platform = "Twitter", Url = "https://twitter.com/msa_official" }
        ]
    };
}

#endregion

#region Sport Assignment Request Examples

/// <summary>
/// Swagger request example for <see cref="AssignAcademySportRequest"/>.
/// </summary>
public class AssignAcademySportRequestExample : IExamplesProvider<AssignAcademySportRequest>
{
    public AssignAcademySportRequest GetExamples() => new()
    {
        SportId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        IsPrimarySport = true
    };
}

#endregion

#region Coach/Athlete Request Examples

/// <summary>
/// Swagger request example for <see cref="AssignCoachToAcademyRequest"/>.
/// </summary>
public class AssignCoachToAcademyRequestExample : IExamplesProvider<AssignCoachToAcademyRequest>
{
    public AssignCoachToAcademyRequest GetExamples() => new()
    {
        CoachId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890")
    };
}

/// <summary>
/// Swagger request example for <see cref="RegisterAthleteWithAcademyRequest"/>.
/// </summary>
public class RegisterAthleteWithAcademyRequestExample : IExamplesProvider<RegisterAthleteWithAcademyRequest>
{
    public RegisterAthleteWithAcademyRequest GetExamples() => new()
    {
        AthleteId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890")
    };
}

/// <summary>
/// Swagger request example for <see cref="TransferAthleteRequest"/>.
/// </summary>
public class TransferAthleteRequestExample : IExamplesProvider<TransferAthleteRequest>
{
    public TransferAthleteRequest GetExamples() => new()
    {
        ToAcademyId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901")
    };
}

#endregion

#region Response Examples

/// <summary>
/// Swagger response example for <see cref="AcademyDto"/>.
/// </summary>
public class AcademyDtoExample : IExamplesProvider<AcademyDto>
{
    public AcademyDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyCode = "ACAD-20250615-A1B2",
        Name = "Mumbai Sports Academy",
        Description = "Premier multi-sport academy in Mumbai.",
        Email = "info@mumbaisportsacademy.com",
        Phone = "+919876543210",
        Status = "Active",
        VerificationStatus = "Verified",
        CreatedAt = DateTime.UtcNow.AddDays(-30),
        Branches = [],
        Sports = [],
        Facilities = [],
        Memberships = [],
        SocialLinks = []
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademyStatisticsDto"/>.
/// </summary>
public class AcademyStatisticsDtoExample : IExamplesProvider<AcademyStatisticsDto>
{
    public AcademyStatisticsDto GetExamples() => new()
    {
        AcademyId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyName = "Mumbai Sports Academy",
        TotalCoaches = 15,
        TotalAthletes = 200,
        TotalBranches = 3,
        TotalFacilities = 12,
        ActiveMemberships = 5,
        SportsOffered = 8,
        TotalDocuments = 20,
        TotalGalleryImages = 50
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademySearchResponse"/>.
/// </summary>
public class AcademySearchResponseExample : IExamplesProvider<AcademySearchResponse>
{
    public AcademySearchResponse GetExamples() => new()
    {
        Items =
        [
            new AcademySummaryDto
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                AcademyCode = "ACAD-20250615-A1B2",
                Name = "Mumbai Sports Academy",
                Description = "Premier multi-sport academy.",
                Email = "info@mumbaisportsacademy.com",
                Phone = "+919876543210",
                Status = "Active",
                VerificationStatus = "Verified",
                City = "Mumbai",
                State = "Maharashtra",
                TotalBranches = 3,
                TotalFacilities = 12,
                TotalSports = 8,
                TotalMemberships = 5,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        ],
        TotalRecords = 1,
        TotalPages = 1,
        CurrentPage = 1,
        PageSize = 20
    };
}

/// <summary>
/// Swagger response example for <see cref="BranchDto"/>.
/// </summary>
public class BranchDtoExample : IExamplesProvider<BranchDto>
{
    public BranchDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        BranchName = "Andheri Branch",
        Address = "123 Sports Avenue, Andheri West",
        City = "Mumbai",
        State = "Maharashtra",
        Country = "India",
        PostalCode = "400058",
        CreatedAt = DateTime.UtcNow.AddDays(-15)
    };
}

/// <summary>
/// Swagger response example for <see cref="FacilityDto"/>.
/// </summary>
public class FacilityDtoExample : IExamplesProvider<FacilityDto>
{
    public FacilityDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        FacilityName = "Main Cricket Ground",
        FacilityType = "Field",
        IndoorOutdoor = "Outdoor",
        Capacity = 200,
        Available = true,
        Description = "Floodlit cricket ground with turf wicket.",
        CreatedAt = DateTime.UtcNow.AddDays(-10)
    };
}

/// <summary>
/// Swagger response example for <see cref="MembershipPlanDto"/>.
/// </summary>
public class MembershipPlanDtoExample : IExamplesProvider<MembershipPlanDto>
{
    public MembershipPlanDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        MembershipName = "Gold Monthly Plan",
        Description = "Unlimited access to all facilities for 30 days.",
        Price = 2500.00m,
        Duration = 30,
        Benefits = "Unlimited gym access, 2 coaching sessions, locker access",
        Status = "Active",
        CreatedAt = DateTime.UtcNow.AddDays(-5)
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademyCoachSummaryDto"/>.
/// </summary>
public class AcademyCoachSummaryDtoExample : IExamplesProvider<AcademyCoachSummaryDto>
{
    public AcademyCoachSummaryDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        CoachId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        CoachCode = "COACH-20250615-X1Y2",
        FullName = "Vikram Singh",
        CoachingLevel = "Senior",
        Status = "Active",
        VerificationStatus = "Verified",
        YearsOfExperience = 10,
        AssignedDate = DateTime.UtcNow.AddDays(-20)
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademyAthleteSummaryDto"/>.
/// </summary>
public class AcademyAthleteSummaryDtoExample : IExamplesProvider<AcademyAthleteSummaryDto>
{
    public AcademyAthleteSummaryDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AthleteId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        AthleteCode = "ATH-20250615-P1Q2",
        FullName = "Priya Sharma",
        CurrentLevel = "Intermediate",
        Status = "Active",
        RegisteredDate = DateTime.UtcNow.AddDays(-10)
    };
}

#endregion
