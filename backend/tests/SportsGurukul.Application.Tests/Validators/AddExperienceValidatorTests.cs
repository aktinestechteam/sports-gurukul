using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class AddExperienceValidatorTests
{
    private readonly AddExperienceValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Academy",
            Role = "Head Coach",
            Sport = "Cricket",
            StartDate = new DateTime(2020, 1, 1),
            EndDate = new DateTime(2023, 12, 31),
            Description = "Led the team to victory."
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.Empty,
            Organization = "Test Academy",
            StartDate = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task EmptyOrganization_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = string.Empty,
            StartDate = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization is required.");
    }

    [Fact]
    public async Task OrganizationExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = new string('x', 201),
            StartDate = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization must not exceed 200 characters.");
    }

    [Fact]
    public async Task RoleExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Academy",
            Role = new string('x', 201),
            StartDate = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Role must not exceed 200 characters.");
    }

    [Fact]
    public async Task SportExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Academy",
            Sport = new string('x', 101),
            StartDate = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Sport)
            .WithErrorMessage("Sport must not exceed 100 characters.");
    }

    [Fact]
    public async Task EndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Academy",
            StartDate = new DateTime(2023, 12, 31),
            EndDate = new DateTime(2020, 1, 1)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date must be after start date.");
    }

    [Fact]
    public async Task DescriptionExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Academy",
            StartDate = DateTime.UtcNow,
            Description = new string('x', 2001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 2000 characters.");
    }
}
