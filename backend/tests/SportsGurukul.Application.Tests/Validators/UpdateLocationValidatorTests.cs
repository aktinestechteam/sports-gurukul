using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateLocation;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateLocationValidatorTests
{
    private readonly UpdateLocationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Latitude = 28.6139m,
            Longitude = 77.2090m,
            Country = "India",
            State = "Delhi",
            City = "New Delhi",
            District = "New Delhi District"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.Empty,
            Latitude = 28.6139m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task LatitudeAboveMax_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Latitude = 95m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90.");
    }

    [Fact]
    public async Task LatitudeBelowMin_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Latitude = -95m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90.");
    }

    [Fact]
    public async Task LongitudeAboveMax_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Longitude = 200m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180.");
    }

    [Fact]
    public async Task LongitudeBelowMin_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Longitude = -200m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180.");
    }

    [Fact]
    public async Task CountryExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Country = new string('x', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country must not exceed 100 characters.");
    }

    [Fact]
    public async Task StateExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            State = new string('x', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.State)
            .WithErrorMessage("State must not exceed 100 characters.");
    }

    [Fact]
    public async Task CityExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            City = new string('x', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City must not exceed 100 characters.");
    }

    [Fact]
    public async Task DistrictExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            District = new string('x', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.District)
            .WithErrorMessage("District must not exceed 100 characters.");
    }

    [Fact]
    public async Task LatitudeInRange_ShouldNotHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Latitude = 90m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public async Task LongitudeInRange_ShouldNotHaveValidationError()
    {
        var command = new UpdateLocationCommand
        {
            CoachId = Guid.NewGuid(),
            Longitude = 180m
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Longitude);
    }
}
