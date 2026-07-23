using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RecordRecentSearch;
using SportsGurukul.Application.Features.AthleteManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class RecordRecentSearchValidatorTests
{
    private readonly RecordRecentSearchValidator _validator = new();

    [Fact]
    public void UserId_Empty_ReturnsError()
    {
        var command = new RecordRecentSearchCommand { UserId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void UserId_Valid_NoError()
    {
        var command = new RecordRecentSearchCommand { UserId = Guid.NewGuid() };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void QueryText_Exceeds500Characters_ReturnsError()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            QueryText = new string('a', 501)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.QueryText)
            .WithErrorMessage("Query text must not exceed 500 characters.");
    }

    [Fact]
    public void QueryText_NullOrWhitespace_SkipsLengthValidation()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            QueryText = null!,
            FiltersJson = "{}"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.QueryText);
    }

    [Fact]
    public void QueryText_WithinLimit_NoError()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            QueryText = new string('a', 500)
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.QueryText);
    }

    [Fact]
    public void FiltersJson_Exceeds4000Characters_ReturnsError()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            FiltersJson = new string('x', 4001)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FiltersJson)
            .WithErrorMessage("Filters must not exceed 4000 characters.");
    }

    [Fact]
    public void FiltersJson_WithinLimit_NoError()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            FiltersJson = new string('x', 4000)
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FiltersJson);
    }

    [Fact]
    public void ValidCommand_NoErrors()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            QueryText = "search term",
            FiltersJson = "{}",
            ResultCount = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyFiltersJson_NoError()
    {
        var command = new RecordRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            FiltersJson = "{}"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FiltersJson);
    }
}
