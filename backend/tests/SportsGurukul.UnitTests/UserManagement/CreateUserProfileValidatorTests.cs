using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;
using SportsGurukul.Application.Features.UserManagement.Validators;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class CreateUserProfileValidatorTests
{
    private readonly CreateUserProfileValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidRequest()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Gender = Gender.Male,
            Bio = "Test bio"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UserIdIsEmpty()
    {
        var command = new CreateUserProfileCommand { UserId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_BioExceeds2000Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Bio = new string('a', 2001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio must not exceed 2000 characters.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_BioIs2000Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Bio = new string('a', 2000)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_HeightExceeds20Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Height = new string('a', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_WeightExceeds20Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Weight = new string('a', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Weight)
            .WithErrorMessage("Weight must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PreferredSportExceeds100Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            PreferredSport = new string('a', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PreferredSport)
            .WithErrorMessage("Preferred sport must not exceed 100 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_ExperienceLevelExceeds50Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            ExperienceLevel = new string('a', 51)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExperienceLevel)
            .WithErrorMessage("Experience level must not exceed 50 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PhoneNumberExceeds15Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            PrimaryPhoneNumber = new string('1', 16)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PrimaryPhoneNumber)
            .WithErrorMessage("Phone number must not exceed 15 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_CountryCodeExceeds5Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            PrimaryPhoneCountryCode = new string('+', 6)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PrimaryPhoneCountryCode)
            .WithErrorMessage("Country code must not exceed 5 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_AddressLine1Exceeds200Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            AddressLine1 = new string('a', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressLine1)
            .WithErrorMessage("Address line 1 must not exceed 200 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_CityExceeds100Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            City = new string('a', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City must not exceed 100 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PostalCodeExceeds20Characters()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            PostalCode = new string('1', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PostalCode)
            .WithErrorMessage("Postal code must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PhoneRequired_When_AddressProvided()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            AddressLine1 = "123 Main St",
            City = "Mumbai",
            PrimaryPhoneNumber = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PrimaryPhoneNumber)
            .WithErrorMessage("Phone number is required when address is provided.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_PhoneProvided_WithAddress()
    {
        var command = new CreateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            AddressLine1 = "123 Main St",
            City = "Mumbai",
            PrimaryPhoneNumber = "1234567890"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PrimaryPhoneNumber);
    }
}
