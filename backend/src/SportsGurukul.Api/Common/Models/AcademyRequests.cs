using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

#region Academy

/// <summary>
/// Request body for creating a new academy.
/// </summary>
public class CreateAcademyRequest
{
    /// <summary>Academy name.</summary>
    /// <example>Mumbai Sports Academy</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Legal registered name of the academy.</summary>
    /// <example>Mumbai Sports Academy Pvt. Ltd.</example>
    public string? LegalName { get; set; }

    /// <summary>Brief description of the academy.</summary>
    /// <example>Premier multi-sport academy in Mumbai.</example>
    public string? Description { get; set; }

    /// <summary>Registration or incorporation number.</summary>
    /// <example>REG-2025-001234</example>
    public string? RegistrationNumber { get; set; }

    /// <summary>GST identification number.</summary>
    /// <example>27AABCU9603R1ZM</example>
    public string? GSTNumber { get; set; }

    /// <summary>Date the academy was established.</summary>
    /// <example>2020-06-15</example>
    public DateTime? EstablishedDate { get; set; }

    /// <summary>Academy website URL.</summary>
    /// <example>https://mumbaisportsacademy.com</example>
    public string? Website { get; set; }

    /// <summary>Primary contact email.</summary>
    /// <example>info@mumbaisportsacademy.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>Primary contact phone.</summary>
    /// <example>+919876543210</example>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Type of the academy. Supported values: SingleSport, MultiSport.</summary>
    /// <example>MultiSport</example>
    public string? AcademyType { get; set; }

    /// <summary>Primary contact person's name.</summary>
    /// <example>Rajesh Kumar</example>
    public string? PrimaryContactName { get; set; }

    /// <summary>Street address of the academy.</summary>
    /// <example>123 Sports Avenue, Andheri West</example>
    public string? Address { get; set; }

    /// <summary>Country of the academy.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>State or province of the academy.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>City of the academy.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Postal code of the academy.</summary>
    /// <example>400058</example>
    public string? PostalCode { get; set; }

    /// <summary>Names of the sports offered by the academy. The first entry is treated as the academy's primary sport.</summary>
    /// <example>["Cricket", "Football"]</example>
    public List<string> SportNames { get; set; } = [];
}

/// <summary>
/// Request body for updating an academy.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateAcademyRequest
{
    /// <summary>Updated academy name.</summary>
    /// <example>Mumbai Sports Academy 2.0</example>
    public string? Name { get; set; }

    /// <summary>Updated legal name.</summary>
    /// <example>MSA Holdings Pvt. Ltd.</example>
    public string? LegalName { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>One of Mumbai's leading multi-sport academies.</example>
    public string? Description { get; set; }

    /// <summary>Updated registration number.</summary>
    /// <example>REG-2025-005678</example>
    public string? RegistrationNumber { get; set; }

    /// <summary>Updated GST number.</summary>
    /// <example>27AABCU9603R1ZN</example>
    public string? GSTNumber { get; set; }

    /// <summary>Updated establishment date.</summary>
    /// <example>2019-01-01</example>
    public DateTime? EstablishedDate { get; set; }

    /// <summary>Updated website.</summary>
    /// <example>https://msa-new.com</example>
    public string? Website { get; set; }

    /// <summary>Updated primary email.</summary>
    /// <example>contact@msa-new.com</example>
    public string? Email { get; set; }

    /// <summary>Updated primary phone.</summary>
    /// <example>+919876543211</example>
    public string? Phone { get; set; }

    /// <summary>Updated logo URL.</summary>
    /// <example>https://cdn.sportsgurukul.com/logos/msa.png</example>
    public string? LogoUrl { get; set; }

    /// <summary>Updated banner URL.</summary>
    /// <example>https://cdn.sportsgurukul.com/banners/msa.jpg</example>
    public string? BannerUrl { get; set; }
}

/// <summary>
/// Request body for verifying an academy.
/// </summary>
public class VerifyAcademyRequest
{
    /// <summary>Verification remarks.</summary>
    /// <example>Academy documentation verified successfully.</example>
    public string? Remarks { get; set; }
}

/// <summary>
/// Request body for rejecting an academy verification.
/// </summary>
public class RejectAcademyVerificationRequest
{
    /// <summary>Reason for rejection (required).</summary>
    /// <example>Registration document is expired. Please update and resubmit.</example>
    public string Remarks { get; set; } = string.Empty;
}

#endregion

#region Branch

/// <summary>
/// Request body for creating a new branch under an academy.
/// </summary>
public class CreateBranchRequest
{
    /// <summary>Branch name.</summary>
    /// <example>Andheri Branch</example>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>Street address.</summary>
    /// <example>123 Sports Avenue, Andheri West</example>
    public string? Address { get; set; }

    /// <summary>Country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>State or province.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>City.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>District.</summary>
    /// <example>Andheri</example>
    public string? District { get; set; }

    /// <summary>Postal code.</summary>
    /// <example>400058</example>
    public string? PostalCode { get; set; }

    /// <summary>Latitude coordinate.</summary>
    /// <example>19.1364</example>
    public decimal? Latitude { get; set; }

    /// <summary>Longitude coordinate.</summary>
    /// <example>72.8296</example>
    public decimal? Longitude { get; set; }
}

/// <summary>
/// Request body for updating a branch.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateBranchRequest
{
    /// <summary>Updated branch name.</summary>
    /// <example>Andheri West Branch</example>
    public string? BranchName { get; set; }

    /// <summary>Updated address.</summary>
    /// <example>456 Sports Lane, Andheri West</example>
    public string? Address { get; set; }

    /// <summary>Updated country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>Updated state.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Updated city.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Updated district.</summary>
    /// <example>Andheri</example>
    public string? District { get; set; }

    /// <summary>Updated postal code.</summary>
    /// <example>400058</example>
    public string? PostalCode { get; set; }

    /// <summary>Updated latitude.</summary>
    /// <example>19.1364</example>
    public decimal? Latitude { get; set; }

    /// <summary>Updated longitude.</summary>
    /// <example>72.8296</example>
    public decimal? Longitude { get; set; }
}

#endregion

#region Facility

/// <summary>
/// Request body for creating a new facility under an academy.
/// </summary>
public class CreateFacilityRequest
{
    /// <summary>Facility name.</summary>
    /// <example>Main Cricket Ground</example>
    public string FacilityName { get; set; } = string.Empty;

    /// <summary>Type of facility.</summary>
    /// <example>Field</example>
    public AcademyFacilityType FacilityType { get; set; } = AcademyFacilityType.Other;

    /// <summary>Indoor or outdoor designation.</summary>
    /// <example>Outdoor</example>
    public string? IndoorOutdoor { get; set; }

    /// <summary>Maximum capacity of the facility.</summary>
    /// <example>200</example>
    public int? Capacity { get; set; }

    /// <summary>Whether the facility is currently available.</summary>
    /// <example>true</example>
    public bool Available { get; set; } = true;

    /// <summary>Brief description of the facility.</summary>
    /// <example>Floodlit cricket ground with turf wicket.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for updating a facility.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateFacilityRequest
{
    /// <summary>Updated facility name.</summary>
    /// <example>Cricket Ground - North</example>
    public string? FacilityName { get; set; }

    /// <summary>Updated facility type.</summary>
    /// <example>Field</example>
    public AcademyFacilityType? FacilityType { get; set; }

    /// <summary>Updated indoor/outdoor designation.</summary>
    /// <example>Indoor</example>
    public string? IndoorOutdoor { get; set; }

    /// <summary>Updated capacity.</summary>
    /// <example>250</example>
    public int? Capacity { get; set; }

    /// <summary>Updated availability status.</summary>
    /// <example>false</example>
    public bool? Available { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Floodlit cricket ground with two turf wickets.</example>
    public string? Description { get; set; }
}

#endregion

#region Membership

/// <summary>
/// Request body for creating a new membership plan under an academy.
/// </summary>
public class CreateMembershipPlanRequest
{
    /// <summary>Membership plan name.</summary>
    /// <example>Gold Monthly Plan</example>
    public string MembershipName { get; set; } = string.Empty;

    /// <summary>Brief description of the plan.</summary>
    /// <example>Unlimited access to all facilities for 30 days.</example>
    public string? Description { get; set; }

    /// <summary>Price of the membership in INR.</summary>
    /// <example>2500.00</example>
    public decimal Price { get; set; }

    /// <summary>Duration of the membership in days.</summary>
    /// <example>30</example>
    public int Duration { get; set; }

    /// <summary>Benefits included in the membership.</summary>
    /// <example>Unlimited gym access, 2 coaching sessions, locker access</example>
    public string? Benefits { get; set; }
}

/// <summary>
/// Request body for updating a membership plan.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateMembershipPlanRequest
{
    /// <summary>Updated plan name.</summary>
    /// <example>Gold Monthly Plan v2</example>
    public string? MembershipName { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Enhanced monthly plan with additional perks.</example>
    public string? Description { get; set; }

    /// <summary>Updated price.</summary>
    /// <example>3000.00</example>
    public decimal? Price { get; set; }

    /// <summary>Updated duration in days.</summary>
    /// <example>30</example>
    public int? Duration { get; set; }

    /// <summary>Updated benefits.</summary>
    /// <example>Unlimited access, 4 coaching sessions, locker, physio</example>
    public string? Benefits { get; set; }
}

#endregion

#region Contact

/// <summary>
/// Request body for updating academy contact information.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateAcademyContactRequest
{
    /// <summary>Primary contact person name.</summary>
    /// <example>Rajesh Kumar</example>
    public string? PrimaryContactName { get; set; }

    /// <summary>Primary phone number.</summary>
    /// <example>+919876543210</example>
    public string? PrimaryPhone { get; set; }

    /// <summary>Primary email address.</summary>
    /// <example>rajesh@mumbaisportsacademy.com</example>
    public string? PrimaryEmail { get; set; }

    /// <summary>Secondary contact person name.</summary>
    /// <example>Priya Sharma</example>
    public string? SecondaryContactName { get; set; }

    /// <summary>Secondary phone number.</summary>
    /// <example>+919876543211</example>
    public string? SecondaryPhone { get; set; }

    /// <summary>Secondary email address.</summary>
    /// <example>priya@mumbaisportsacademy.com</example>
    public string? SecondaryEmail { get; set; }

    /// <summary>Contact address.</summary>
    /// <example>123 Sports Avenue, Andheri West, Mumbai</example>
    public string? Address { get; set; }

    /// <summary>Country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>State.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>City.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Postal code.</summary>
    /// <example>400058</example>
    public string? PostalCode { get; set; }

    /// <summary>Latitude coordinate.</summary>
    /// <example>19.1364</example>
    public decimal? Latitude { get; set; }

    /// <summary>Longitude coordinate.</summary>
    /// <example>72.8296</example>
    public decimal? Longitude { get; set; }
}

#endregion

#region Operating Hours

/// <summary>
/// Request body for updating academy operating hours.
/// All fields are optional — only supplied fields are applied.
/// Use null to indicate a closed day.
/// </summary>
public class UpdateOperatingHoursRequest
{
    /// <summary>Monday opening time (HH:mm format).</summary>
    /// <example>06:00</example>
    public string? MondayOpening { get; set; }

    /// <summary>Monday closing time (HH:mm format).</summary>
    /// <example>21:00</example>
    public string? MondayClosing { get; set; }

    /// <summary>Tuesday opening time.</summary>
    /// <example>06:00</example>
    public string? TuesdayOpening { get; set; }

    /// <summary>Tuesday closing time.</summary>
    /// <example>21:00</example>
    public string? TuesdayClosing { get; set; }

    /// <summary>Wednesday opening time.</summary>
    /// <example>06:00</example>
    public string? WednesdayOpening { get; set; }

    /// <summary>Wednesday closing time.</summary>
    /// <example>21:00</example>
    public string? WednesdayClosing { get; set; }

    /// <summary>Thursday opening time.</summary>
    /// <example>06:00</example>
    public string? ThursdayOpening { get; set; }

    /// <summary>Thursday closing time.</summary>
    /// <example>21:00</example>
    public string? ThursdayClosing { get; set; }

    /// <summary>Friday opening time.</summary>
    /// <example>06:00</example>
    public string? FridayOpening { get; set; }

    /// <summary>Friday closing time.</summary>
    /// <example>21:00</example>
    public string? FridayClosing { get; set; }

    /// <summary>Saturday opening time.</summary>
    /// <example>07:00</example>
    public string? SaturdayOpening { get; set; }

    /// <summary>Saturday closing time.</summary>
    /// <example>19:00</example>
    public string? SaturdayClosing { get; set; }

    /// <summary>Sunday opening time.</summary>
    /// <example>07:00</example>
    public string? SundayOpening { get; set; }

    /// <summary>Sunday closing time.</summary>
    /// <example>14:00</example>
    public string? SundayClosing { get; set; }

    /// <summary>Holiday schedule description.</summary>
    /// <example>Closed on national holidays. Reduced hours on festival days.</example>
    public string? HolidaySchedule { get; set; }
}

#endregion

#region Social Links

/// <summary>
/// Request body for updating academy social media links.
/// Replaces all existing links with the provided list.
/// </summary>
public class UpdateSocialLinksRequest
{
    /// <summary>List of social media links.</summary>
    public List<SocialLinkInput> Links { get; set; } = [];
}

/// <summary>
/// Individual social media link entry.
/// </summary>
public class SocialLinkInput
{
    /// <summary>Social media platform name.</summary>
    /// <example>Instagram</example>
    public string Platform { get; set; } = string.Empty;

    /// <summary>URL to the social media profile.</summary>
    /// <example>https://instagram.com/mumbaisportsacademy</example>
    public string Url { get; set; } = string.Empty;
}

#endregion

#region Sport Assignment

/// <summary>
/// Request body for assigning a sport to an academy.
/// </summary>
public class AssignAcademySportRequest
{
    /// <summary>Unique identifier of the sport to assign.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid SportId { get; set; }

    /// <summary>Whether this is the academy's primary sport.</summary>
    /// <example>true</example>
    public bool IsPrimarySport { get; set; }
}

#endregion

#region Coach Assignment

/// <summary>
/// Request body for assigning a coach to an academy.
/// </summary>
public class AssignCoachToAcademyRequest
{
    /// <summary>Unique identifier of the coach to assign.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid CoachId { get; set; }
}

#endregion

#region Athlete Registration

/// <summary>
/// Request body for registering an athlete with an academy.
/// </summary>
public class RegisterAthleteWithAcademyRequest
{
    /// <summary>Unique identifier of the athlete to register.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid AthleteId { get; set; }
}

/// <summary>
/// Request body for transferring an athlete between academies.
/// </summary>
public class TransferAthleteRequest
{
    /// <summary>Unique identifier of the destination academy.</summary>
    /// <example>b2c3d4e5-f6a7-8901-bcde-f12345678901</example>
    public Guid ToAcademyId { get; set; }
}

#endregion

#region Academy Search

/// <summary>
/// Query parameters for advanced academy search.
/// </summary>
public class AcademySearchRequest
{
    /// <summary>Free-text search across academy name, code, and description.</summary>
    /// <example>Mumbai</example>
    public string? SearchTerm { get; set; }

    /// <summary>Filter by academy name (partial match).</summary>
    /// <example>Elite</example>
    public string? Name { get; set; }

    /// <summary>Filter by city (partial match).</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Filter by state (partial match).</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Filter by sport name (partial match).</summary>
    /// <example>Cricket</example>
    public string? SportName { get; set; }

    /// <summary>Filter by verification status.</summary>
    /// <example>Verified</example>
    public string? VerificationStatus { get; set; }

    /// <summary>Filter by membership type.</summary>
    /// <example>Gold</example>
    public string? MembershipType { get; set; }

    /// <summary>Filter by facility type.</summary>
    /// <example>Field</example>
    public string? FacilityType { get; set; }

    /// <summary>Page number (1-based, default 1).</summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>Items per page (default 20, max 100).</summary>
    /// <example>20</example>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Query parameters for academy autocomplete suggestions.
/// </summary>
public class AcademySuggestionsRequest
{
    /// <summary>Search prefix for autocomplete.</summary>
    /// <example>Mum</example>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>Maximum number of suggestions to return (default 10).</summary>
    /// <example>10</example>
    public int Limit { get; set; } = 10;
}

#endregion
