using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteSavedSearch;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RecordRecentSearch;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;

namespace SportsGurukul.Application.Tests.Validators;

public class SearchFeatureValidatorTests
{
    [Fact]
    public async Task CreateSavedSearch_ValidCommand_NoErrors()
    {
        var validator = new CreateSavedSearchValidator();
        var command = new CreateSavedSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = "My Search",
            FiltersJson = "{\"city\":\"Mumbai\"}"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateSavedSearch_EmptyName_ShouldHaveError()
    {
        var validator = new CreateSavedSearchValidator();
        var command = new CreateSavedSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = string.Empty,
            FiltersJson = "{}"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CreateSavedSearch_NameExceedsMaxLength_ShouldHaveError()
    {
        var validator = new CreateSavedSearchValidator();
        var command = new CreateSavedSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = new string('x', 101),
            FiltersJson = "{}"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task DeleteSavedSearch_ValidCommand_NoErrors()
    {
        var validator = new DeleteSavedSearchValidator();
        var command = new DeleteSavedSearchCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeleteSavedSearch_EmptyIds_ShouldHaveErrors()
    {
        var validator = new DeleteSavedSearchValidator();
        var command = new DeleteSavedSearchCommand
        {
            Id = Guid.Empty,
            UserId = Guid.Empty
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task GetSavedSearches_EmptyUserId_ShouldHaveError()
    {
        var validator = new GetSavedSearchesValidator();
        var command = new GetSavedSearchesQuery { UserId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task GetRecentSearches_ValidQuery_NoErrors()
    {
        var validator = new GetRecentSearchesValidator();
        var command = new GetRecentSearchesQuery
        {
            UserId = Guid.NewGuid(),
            Limit = 10
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetRecentSearches_LimitOver50_ShouldHaveError()
    {
        var validator = new GetRecentSearchesValidator();
        var command = new GetRecentSearchesQuery
        {
            UserId = Guid.NewGuid(),
            Limit = 51
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public async Task GetAthleteSuggestions_ValidQuery_NoErrors()
    {
        var validator = new GetAthleteSuggestionsValidator();
        var command = new GetAthleteSuggestionsQuery
        {
            Prefix = "cr",
            Limit = 10
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetAthleteSuggestions_ShortPrefix_ShouldHaveError()
    {
        var validator = new GetAthleteSuggestionsValidator();
        var command = new GetAthleteSuggestionsQuery
        {
            Prefix = "c",
            Limit = 10
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Prefix);
    }

    [Fact]
    public async Task GetAthleteSuggestions_LimitOver25_ShouldHaveError()
    {
        var validator = new GetAthleteSuggestionsValidator();
        var command = new GetAthleteSuggestionsQuery
        {
            Prefix = "cr",
            Limit = 26
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }
}
