using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for creating a new coach profile linked to an existing user account.
/// </summary>
public class CreateCoachRequest
{
    /// <summary>Unique identifier of the user to create the coach profile for.</summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid UserId { get; set; }

    /// <summary>Short biography or description of coaching background.</summary>
    /// <example>Senior cricket coach with 10 years of experience coaching at state level.</example>
    public string? Biography { get; set; }

    /// <summary>Years of coaching experience.</summary>
    /// <example>10</example>
    public int YearsOfExperience { get; set; }

    /// <summary>Current organization or academy name.</summary>
    /// <example>Mumbai Cricket Academy</example>
    public string? CurrentOrganization { get; set; }

    /// <summary>Highest coaching qualification.</summary>
    /// <example>BCCI Level A Coaching Certificate</example>
    public string? HighestQualification { get; set; }

    /// <summary>Preferred language for communication.</summary>
    /// <example>English</example>
    public string? PreferredLanguage { get; set; }

    /// <summary>Coaching level classification.</summary>
    /// <example>Senior</example>
    public CoachingLevel CoachingLevel { get; set; } = CoachingLevel.Junior;
}

/// <summary>
/// Request body for updating a coach profile.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateCoachProfileRequest
{
    /// <summary>Updated biography.</summary>
    /// <example>Senior cricket coach with 12 years of experience coaching at national level.</example>
    public string? Biography { get; set; }

    /// <summary>Updated years of experience.</summary>
    /// <example>12</example>
    public int? YearsOfExperience { get; set; }

    /// <summary>Updated current organization.</summary>
    /// <example>National Cricket Academy</example>
    public string? CurrentOrganization { get; set; }

    /// <summary>Updated highest qualification.</summary>
    /// <example>BCCI Level B Coaching Certificate</example>
    public string? HighestQualification { get; set; }

    /// <summary>Updated preferred language.</summary>
    /// <example>Hindi</example>
    public string? PreferredLanguage { get; set; }

    /// <summary>Updated coaching level.</summary>
    /// <example>Expert</example>
    public CoachingLevel? CoachingLevel { get; set; }
}

/// <summary>
/// Request body for assigning a sport to a coach.
/// </summary>
public class CoachAssignSportRequest
{
    /// <summary>Unique identifier of the sport to assign.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid SportId { get; set; }

    /// <summary>Whether this is the coach's primary sport.</summary>
    /// <example>true</example>
    public bool IsPrimarySport { get; set; }
}

/// <summary>
/// Request body for adding a certification to a coach profile.
/// </summary>
public class AddCertificationRequest
{
    /// <summary>Name of the certification.</summary>
    /// <example>BCCI Level A Coaching Certificate</example>
    public string CertificationName { get; set; } = string.Empty;

    /// <summary>Authority that issued the certification.</summary>
    /// <example>Board of Control for Cricket in India</example>
    public string? IssuingAuthority { get; set; }

    /// <summary>Certificate number or ID.</summary>
    /// <example>BCCI-LA-2024-001</example>
    public string? CertificateNumber { get; set; }

    /// <summary>Date the certification was issued.</summary>
    /// <example>2024-01-15</example>
    public DateTime? IssueDate { get; set; }

    /// <summary>Date the certification expires.</summary>
    /// <example>2027-01-15</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>URL to the certification document.</summary>
    /// <example>https://cdn.sportsgurukul.com/certificates/coach-001.pdf</example>
    public string? CertificateUrl { get; set; }
}

/// <summary>
/// Request body for updating a coach certification.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateCertificationRequest
{
    /// <summary>Updated certification name.</summary>
    /// <example>BCCI Level B Coaching Certificate</example>
    public string? CertificationName { get; set; }

    /// <summary>Updated issuing authority.</summary>
    /// <example>Board of Control for Cricket in India</example>
    public string? IssuingAuthority { get; set; }

    /// <summary>Updated certificate number.</summary>
    /// <example>BCCI-LB-2025-002</example>
    public string? CertificateNumber { get; set; }

    /// <summary>Updated issue date.</summary>
    /// <example>2025-03-01</example>
    public DateTime? IssueDate { get; set; }

    /// <summary>Updated expiry date.</summary>
    /// <example>2028-03-01</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Updated certificate URL.</summary>
    /// <example>https://cdn.sportsgurukul.com/certificates/coach-002.pdf</example>
    public string? CertificateUrl { get; set; }
}

/// <summary>
/// Request body for verifying a coach certification.
/// </summary>
public class VerifyCertificationRequest
{
    /// <summary>Verification status to set.</summary>
    /// <example>Verified</example>
    public VerificationStatus Status { get; set; }
}

/// <summary>
/// Request body for adding experience to a coach profile.
/// </summary>
public class AddExperienceRequest
{
    /// <summary>Organization name.</summary>
    /// <example>Mumbai Cricket Academy</example>
    public string Organization { get; set; } = string.Empty;

    /// <summary>Role held at the organization.</summary>
    /// <example>Head Coach</example>
    public string? Role { get; set; }

    /// <summary>Sport coached.</summary>
    /// <example>Cricket</example>
    public string? Sport { get; set; }

    /// <summary>Start date of the experience.</summary>
    /// <example>2020-01-15</example>
    public DateTime StartDate { get; set; }

    /// <summary>End date (null if current).</summary>
    /// <example>2024-06-30</example>
    public DateTime? EndDate { get; set; }

    /// <summary>Description of responsibilities and achievements.</summary>
    /// <example>Coached under-19 cricket team to state championship finals.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for updating a coach experience entry.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateExperienceRequest
{
    /// <summary>Updated organization name.</summary>
    /// <example>National Cricket Academy</example>
    public string? Organization { get; set; }

    /// <summary>Updated role.</summary>
    /// <example>Assistant Coach</example>
    public string? Role { get; set; }

    /// <summary>Updated sport.</summary>
    /// <example>Cricket</example>
    public string? Sport { get; set; }

    /// <summary>Updated start date.</summary>
    /// <example>2019-06-01</example>
    public DateTime? StartDate { get; set; }

    /// <summary>Updated end date.</summary>
    /// <example>2024-12-31</example>
    public DateTime? EndDate { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Led national junior team training program.</example>
    public string? Description { get; set; }
}

/// <summary>
/// Request body for adding education to a coach profile.
/// </summary>
public class AddEducationRequest
{
    /// <summary>Degree or certification name.</summary>
    /// <example>Bachelor of Physical Education</example>
    public string Degree { get; set; } = string.Empty;

    /// <summary>Institution name.</summary>
    /// <example>National Institute of Sports</example>
    public string? Institution { get; set; }

    /// <summary>Field of study.</summary>
    /// <example>Sports Coaching</example>
    public string? FieldOfStudy { get; set; }

    /// <summary>Year of completion.</summary>
    /// <example>2018</example>
    public int? YearCompleted { get; set; }
}

/// <summary>
/// Request body for updating a coach education entry.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateEducationRequest
{
    /// <summary>Updated degree.</summary>
    /// <example>Master of Sports Science</example>
    public string? Degree { get; set; }

    /// <summary>Updated institution.</summary>
    /// <example>Loughborough University</example>
    public string? Institution { get; set; }

    /// <summary>Updated field of study.</summary>
    /// <example>Sports Science</example>
    public string? FieldOfStudy { get; set; }

    /// <summary>Updated year of completion.</summary>
    /// <example>2020</example>
    public int? YearCompleted { get; set; }
}

/// <summary>
/// Request body for updating a coach's availability schedule.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateAvailabilityRequest
{
    /// <summary>Weekly schedule as a JSON string.</summary>
    /// <example>{"Monday":"06:00-18:00","Tuesday":"06:00-18:00","Wednesday":"06:00-18:00","Thursday":"06:00-18:00","Friday":"06:00-18:00","Saturday":"08:00-14:00"}</example>
    public string? WeeklySchedule { get; set; }

    /// <summary>Available time slots as a JSON string.</summary>
    /// <example>["06:00-08:00","08:00-10:00","10:00-12:00","14:00-16:00","16:00-18:00"]</example>
    public string? TimeSlots { get; set; }

    /// <summary>Whether online coaching sessions are available.</summary>
    /// <example>true</example>
    public bool? OnlineAvailable { get; set; }

    /// <summary>Whether in-person coaching is available.</summary>
    /// <example>true</example>
    public bool? OfflineAvailable { get; set; }

    /// <summary>Maximum travel distance in km for in-person coaching.</summary>
    /// <example>25</example>
    public int? TravelDistance { get; set; }
}

/// <summary>
/// Request body for updating a coach's location.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateLocationRequest
{
    /// <summary>Country name.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>State or province.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>City name.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>District name.</summary>
    /// <example>Mumbai City</example>
    public string? District { get; set; }

    /// <summary>Latitude coordinate (range: -90 to 90).</summary>
    /// <example>19.0760</example>
    public decimal? Latitude { get; set; }

    /// <summary>Longitude coordinate (range: -180 to 180).</summary>
    /// <example>72.8777</example>
    public decimal? Longitude { get; set; }
}
