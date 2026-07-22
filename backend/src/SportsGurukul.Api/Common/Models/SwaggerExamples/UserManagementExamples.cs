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
        Gender = Domain.Enums.Gender.Male,
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
        Theme = Domain.Enums.Theme.Dark,
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
