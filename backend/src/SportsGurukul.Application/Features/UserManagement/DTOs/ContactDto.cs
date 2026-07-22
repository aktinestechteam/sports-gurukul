namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
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
}
