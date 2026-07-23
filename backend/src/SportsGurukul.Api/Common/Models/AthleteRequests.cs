using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for creating a new athlete profile.
/// The athlete profile is linked to an existing user account via <c>UserId</c>.
/// </summary>
public class CreateAthleteRequest
{
    /// <summary>Unique identifier of the user to create the athlete profile for.</summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid UserId { get; set; }

    /// <summary>Current skill level of the athlete.</summary>
    /// <example>Intermediate</example>
    public AthleteLevel CurrentLevel { get; set; } = AthleteLevel.Beginner;

    /// <summary>Years of sports experience.</summary>
    /// <example>5</example>
    public int ExperienceYears { get; set; }

    /// <summary>Height in human-readable form.</summary>
    /// <example>5'10"</example>
    public string? Height { get; set; }

    /// <summary>Weight in human-readable form.</summary>
    /// <example>75kg</example>
    public string? Weight { get; set; }

    /// <summary>Blood group.</summary>
    /// <example>O Positive</example>
    public BloodGroup? BloodGroup { get; set; }

    /// <summary>Dominant hand for racquet sports.</summary>
    /// <example>Right</example>
    public DominantHand? DominantHand { get; set; }

    /// <summary>Dominant foot for field sports.</summary>
    /// <example>Right</example>
    public DominantFoot? DominantFoot { get; set; }

    /// <summary>Short biography or description.</summary>
    /// <example>Passionate cricket player with 5 years of experience.</example>
    public string? Biography { get; set; }
}

/// <summary>
/// Request body for updating an athlete profile.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateAthleteRequest
{
    /// <summary>Updated skill level.</summary>
    /// <example>Advanced</example>
    public AthleteLevel? CurrentLevel { get; set; }

    /// <summary>Updated years of experience.</summary>
    /// <example>7</example>
    public int? ExperienceYears { get; set; }

    /// <summary>Updated height.</summary>
    /// <example>6'0"</example>
    public string? Height { get; set; }

    /// <summary>Updated weight.</summary>
    /// <example>78kg</example>
    public string? Weight { get; set; }

    /// <summary>Updated blood group.</summary>
    /// <example>A Positive</example>
    public BloodGroup? BloodGroup { get; set; }

    /// <summary>Updated dominant hand.</summary>
    /// <example>Left</example>
    public DominantHand? DominantHand { get; set; }

    /// <summary>Updated dominant foot.</summary>
    /// <example>Right</example>
    public DominantFoot? DominantFoot { get; set; }

    /// <summary>Updated biography.</summary>
    /// <example>National-level sprinter focused on 100m and 200m events.</example>
    public string? Biography { get; set; }

    /// <summary>Updated athlete status.</summary>
    /// <example>Active</example>
    public AthleteStatus? Status { get; set; }
}

/// <summary>
/// Request body for assigning a sport to an athlete.
/// </summary>
public class AssignSportRequest
{
    /// <summary>Unique identifier of the sport to assign.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid SportId { get; set; }

    /// <summary>Whether this is the athlete's primary sport.</summary>
    /// <example>true</example>
    public bool IsPrimarySport { get; set; }
}

/// <summary>
/// Request body for adding an achievement to an athlete.
/// </summary>
public class AddAchievementRequest
{
    /// <summary>Title of the achievement.</summary>
    /// <example>State Championship Gold Medal</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Name of the competition or event.</summary>
    /// <example>Maharashtra State Athletics Championship 2025</example>
    public string? Competition { get; set; }

    /// <summary>Position or placement achieved.</summary>
    /// <example>1st Place</example>
    public string? Position { get; set; }

    /// <summary>Level of the achievement.</summary>
    /// <example>State</example>
    public AchievementLevel Level { get; set; } = AchievementLevel.Local;

    /// <summary>Date the achievement was awarded.</summary>
    /// <example>2025-03-15</example>
    public DateTime Date { get; set; }

    /// <summary>URL to the certificate or proof document.</summary>
    /// <example>https://cdn.sportsgurukul.com/certificates/ath-001.pdf</example>
    public string? CertificateUrl { get; set; }

    /// <summary>Additional notes about the achievement.</summary>
    /// <example>First place in the 100m sprint with a personal best time.</example>
    public string? Notes { get; set; }
}

/// <summary>
/// Request body for updating an athlete achievement.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateAchievementRequest
{
    /// <summary>Updated title.</summary>
    /// <example>National Championship Silver Medal</example>
    public string? Title { get; set; }

    /// <summary>Updated competition name.</summary>
    /// <example>National Athletics Championship 2025</example>
    public string? Competition { get; set; }

    /// <summary>Updated position.</summary>
    /// <example>2nd Place</example>
    public string? Position { get; set; }

    /// <summary>Updated level.</summary>
    /// <example>National</example>
    public AchievementLevel? Level { get; set; }

    /// <summary>Updated date.</summary>
    /// <example>2025-07-20</example>
    public DateTime? Date { get; set; }

    /// <summary>Updated certificate URL.</summary>
    /// <example>https://cdn.sportsgurukul.com/certificates/ath-002.pdf</example>
    public string? CertificateUrl { get; set; }

    /// <summary>Updated notes.</summary>
    /// <example>Personal best time in the 200m event.</example>
    public string? Notes { get; set; }
}

/// <summary>
/// Request body for updating an athlete's medical profile.
/// All fields are optional — only supplied fields are applied.
/// Medical information is never logged for privacy compliance.
/// </summary>
public class UpdateMedicalProfileRequest
{
    /// <summary>Known medical conditions.</summary>
    /// <example>Asthma (mild)</example>
    public string? MedicalConditions { get; set; }

    /// <summary>Known allergies.</summary>
    /// <example>Penicillin</example>
    public string? Allergies { get; set; }

    /// <summary>Current medications.</summary>
    /// <example>Inhaler as needed</example>
    public string? Medications { get; set; }

    /// <summary>Blood group for medical reference.</summary>
    /// <example>O Positive</example>
    public string? BloodGroup { get; set; }

    /// <summary>Health insurance policy number.</summary>
    /// <example>INS-2025-001234</example>
    public string? InsuranceNumber { get; set; }

    /// <summary>Name of the primary physician.</summary>
    /// <example>Dr. Anjali Mehta</example>
    public string? DoctorName { get; set; }

    /// <summary>Contact number for the primary physician.</summary>
    /// <example>+919876543210</example>
    public string? DoctorContact { get; set; }
}

/// <summary>
/// Request body for updating an athlete's emergency contact.
/// </summary>
public class UpdateEmergencyContactRequest
{
    /// <summary>Full name of the emergency contact.</summary>
    /// <example>Sunita Sharma</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Relationship to the athlete.</summary>
    /// <example>Mother</example>
    public EmergencyRelationship Relationship { get; set; }

    /// <summary>Phone number of the emergency contact.</summary>
    /// <example>+919876543210</example>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Email address of the emergency contact.</summary>
    /// <example>sunita.sharma@example.com</example>
    public string? Email { get; set; }
}

/// <summary>
/// Request body for updating an athlete's ranking information.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateRankingRequest
{
    /// <summary>Current club or academy rank.</summary>
    /// <example>A+</example>
    public string? CurrentRank { get; set; }

    /// <summary>State-level ranking.</summary>
    /// <example>12</example>
    public string? StateRank { get; set; }

    /// <summary>National ranking.</summary>
    /// <example>156</example>
    public string? NationalRank { get; set; }

    /// <summary>International ranking.</summary>
    /// <example>2500</example>
    public string? InternationalRank { get; set; }

    /// <summary>Authority that assigned the ranking.</summary>
    /// <example>Athletics Federation of India</example>
    public string? RankingAuthority { get; set; }
}
