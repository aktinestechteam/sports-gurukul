using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.PreferNotToSay;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? PreferredSport { get; set; }
    public string? ExperienceLevel { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ContactInformation? ContactInformation { get; set; }
    public UserPreference? UserPreference { get; set; }
}
