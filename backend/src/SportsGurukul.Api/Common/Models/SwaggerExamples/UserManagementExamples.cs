using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger request example for <see cref="UpdateUserProfileRequest"/>.
/// </summary>
public class UpdateUserProfileRequestExample : IExamplesProvider<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequest GetExamples() => new()
    {
        DateOfBirth = new DateTime(2000, 6, 15),
        Gender = Gender.Male,
        Bio = "Passionate cricket player with 5 years of experience.",
        Height = "5'10\"",
        Weight = "75kg",
        PreferredSport = "Cricket",
        ExperienceLevel = "Intermediate",
        PrimaryPhoneCountryCode = "+91",
        PrimaryPhoneNumber = "9876543210",
        AddressLine1 = "123 Sports Avenue",
        City = "Mumbai",
        State = "Maharashtra",
        Country = "India",
        PostalCode = "400001"
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateUserPreferenceRequest"/>.
/// </summary>
public class UpdateUserPreferenceRequestExample : IExamplesProvider<UpdateUserPreferenceRequest>
{
    public UpdateUserPreferenceRequest GetExamples() => new()
    {
        Language = "en",
        Theme = Theme.Dark,
        TimeZone = "Asia/Kolkata",
        EmailNotifications = true,
        PushNotifications = true,
        SmsNotifications = false,
        MarketingEmails = false,
        ProfileVisibility = true,
        ShowOnlineStatus = true
    };
}

/// <summary>
/// Swagger request example for <see cref="RestoreUserProfileRequest"/>.
/// </summary>
public class RestoreUserProfileRequestExample : IExamplesProvider<RestoreUserProfileRequest>
{
    public RestoreUserProfileRequest GetExamples() => new()
    {
        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479")
    };
}

/// <summary>
/// Swagger response example for <see cref="UserSearchResponse"/>.
/// </summary>
public class UserSearchResponseExample : IExamplesProvider<UserSearchResponse>
{
    public UserSearchResponse GetExamples() => new()
    {
        Items =
        [
            new UserListDto
            {
                UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                FullName = "Rahul Sharma",
                Email = "rahul@example.com",
                PhoneNumber = "9876543210",
                ProfileImageUrl = "https://cdn.sportsgurukul.com/photos/rahul.jpg",
                Status = UserStatus.Active,
                IsEmailVerified = true,
                Gender = Gender.Male,
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                PreferredSport = "Cricket",
                Roles = ["Coach"],
                CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 6, 20, 14, 45, 0, DateTimeKind.Utc)
            },
            new UserListDto
            {
                UserId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                FullName = "Priya Patel",
                Email = "priya@example.com",
                PhoneNumber = "9123456789",
                Status = UserStatus.Active,
                IsEmailVerified = true,
                Gender = Gender.Female,
                City = "Delhi",
                State = "Delhi",
                Country = "India",
                PreferredSport = "Badminton",
                Roles = ["Athlete"],
                CreatedAt = new DateTime(2025, 3, 10, 8, 0, 0, DateTimeKind.Utc)
            }
        ],
        TotalRecords = 42,
        TotalPages = 3,
        CurrentPage = 1,
        PageSize = 20
    };
}
