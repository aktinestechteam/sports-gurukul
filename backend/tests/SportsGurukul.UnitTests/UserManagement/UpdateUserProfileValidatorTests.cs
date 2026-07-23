using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserProfile;
using SportsGurukul.Application.Features.UserManagement.Validators;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class UpdateUserProfileValidatorTests
{
    private readonly UpdateUserProfileValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidRequest()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Gender = Gender.Male,
            Bio = "Updated bio"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UserIdIsEmpty()
    {
        var command = new UpdateUserProfileCommand { UserId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_BioExceeds2000Characters()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Bio = new string('a', 2001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio must not exceed 2000 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_HeightExceeds20Characters()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Height = new string('a', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PhoneNumberExceeds15Characters()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            PrimaryPhoneNumber = new string('1', 16)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PrimaryPhoneNumber)
            .WithErrorMessage("Phone number must not exceed 15 characters.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_DateOfBirthIsNull()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            DateOfBirth = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_OptionalFieldsAreNull()
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Bio = null,
            Height = null,
            Weight = null,
            PreferredSport = null,
            ExperienceLevel = null,
            PrimaryPhoneNumber = null,
            AddressLine1 = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
