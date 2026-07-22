using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for updating the current user's profile.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateUserProfileRequest
{
    /// <summary>Date of birth.</summary>
    /// <example>2000-06-15</example>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>Gender.</summary>
    /// <example>Male</example>
    public Gender Gender { get; set; } = Gender.PreferNotToSay;

    /// <summary>Bio / about-me text shown on the profile.</summary>
    /// <example>Passionate cricket player with 5 years of experience.</example>
    public string? Bio { get; set; }

    /// <summary>Height in human-readable form (e.g. "5'10" or "178cm").</summary>
    /// <example>5'10"</example>
    public string? Height { get; set; }

    /// <summary>Weight in human-readable form (e.g. "75kg" or "165lbs").</summary>
    /// <example>75kg</example>
    public string? Weight { get; set; }

    /// <summary>Preferred sport.</summary>
    /// <example>Cricket</example>
    public string? PreferredSport { get; set; }

    /// <summary>Experience level (e.g. Beginner, Intermediate, Advanced, Professional).</summary>
    /// <example>Intermediate</example>
    public string? ExperienceLevel { get; set; }

    /// <summary>Primary phone country code.</summary>
    /// <example>+91</example>
    public string? PrimaryPhoneCountryCode { get; set; }

    /// <summary>Primary phone number (digits only, without country code).</summary>
    /// <example>9876543210</example>
    public string? PrimaryPhoneNumber { get; set; }

    /// <summary>Address line 1.</summary>
    /// <example>123 Sports Avenue</example>
    public string? AddressLine1 { get; set; }

    /// <summary>Address line 2 (apartment, suite, etc.).</summary>
    /// <example>Apt 4B</example>
    public string? AddressLine2 { get; set; }

    /// <summary>City.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>State / province.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>Postal / ZIP code.</summary>
    /// <example>400001</example>
    public string? PostalCode { get; set; }

    /// <summary>Address type classification.</summary>
    /// <example>Home</example>
    public AddressType AddressType { get; set; } = AddressType.Home;
}
