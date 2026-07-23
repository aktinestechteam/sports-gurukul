using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;
using SportsGurukul.Application.Features.UserManagement.Validators;

namespace SportsGurukul.UnitTests.UserManagement;

public class UpdateUserPreferenceValidatorTests
{
    private readonly UpdateUserPreferenceValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidRequest()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Language = "hi",
            TimeZone = "Asia/Kolkata"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UserIdIsEmpty()
    {
        var command = new UpdateUserPreferenceCommand { UserId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_LanguageExceeds10Characters()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Language = new string('a', 11)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Language)
            .WithErrorMessage("Language code must not exceed 10 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_TimeZoneExceeds100Characters()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            TimeZone = new string('a', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TimeZone)
            .WithErrorMessage("Time zone must not exceed 100 characters.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_LanguageIsNull()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Language = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Language);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_TimeZoneIsNull()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            TimeZone = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TimeZone);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_AllOptionalFieldsNull()
    {
        var command = new UpdateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Language = null,
            Theme = null,
            TimeZone = null,
            EmailNotifications = null,
            PushNotifications = null,
            SmsNotifications = null,
            MarketingEmails = null,
            ProfileVisibility = null,
            ShowOnlineStatus = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
