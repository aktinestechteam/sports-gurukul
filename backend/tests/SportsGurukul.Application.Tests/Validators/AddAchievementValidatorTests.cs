using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class AddAchievementValidatorTests
{
    private readonly AddAchievementValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = "State Championship",
            Competition = "Cricket",
            Position = "1st",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-30),
            CertificateUrl = "https://example.com/cert.pdf",
            Notes = "Great"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.Empty,
            Title = "Test",
            Date = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }

    [Fact]
    public async Task EmptyTitle_ShouldHaveValidationError()
    {
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = string.Empty,
            Date = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public async Task TitleExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = new string('x', 201),
            Date = DateTime.UtcNow
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters.");
    }

    [Fact]
    public async Task FutureDate_ShouldHaveValidationError()
    {
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = "Test",
            Date = DateTime.UtcNow.AddDays(1)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Date)
            .WithErrorMessage("Date cannot be in the future.");
    }
}
