using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class ContactInformation : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public string? PrimaryPhoneCountryCode { get; set; }
    public string? PrimaryPhoneNumber { get; set; }
    public bool PrimaryPhoneVerified { get; set; }
    public string? SecondaryPhoneCountryCode { get; set; }
    public string? SecondaryPhoneNumber { get; set; }
    public bool SecondaryPhoneVerified { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }

    public UserProfile UserProfile { get; set; } = null!;

    public PhoneNumber? GetPrimaryPhoneNumber()
    {
        if (string.IsNullOrEmpty(PrimaryPhoneCountryCode) || string.IsNullOrEmpty(PrimaryPhoneNumber))
            return null;

        return new PhoneNumber(PrimaryPhoneCountryCode, PrimaryPhoneNumber, PrimaryPhoneVerified);
    }

    public PhoneNumber? GetSecondaryPhoneNumber()
    {
        if (string.IsNullOrEmpty(SecondaryPhoneCountryCode) || string.IsNullOrEmpty(SecondaryPhoneNumber))
            return null;

        return new PhoneNumber(SecondaryPhoneCountryCode, SecondaryPhoneNumber, SecondaryPhoneVerified);
    }
}
