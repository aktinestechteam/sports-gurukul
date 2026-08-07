using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? PreferredSport { get; set; }
    public string? ExperienceLevel { get; set; }
    public UserStatus Status { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ProfileCompletionPercentage { get; set; }
    public IReadOnlyList<AddressDto> Addresses { get; set; } = [];
    public ContactDto? ContactInformation { get; set; }
    public UserPreferenceDto? Preferences { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];

    /// <summary>
    /// Whether the user has a completed <c>UserProfile</c> yet. False when the
    /// endpoint answers with an identity-only payload because no profile
    /// exists; roles are always current regardless of this flag.
    /// </summary>
    public bool HasProfile { get; set; } = true;
}
