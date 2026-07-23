using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Queries.SearchAthletes;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class SearchAthletesValidatorTests
{
    private readonly SearchAthletesValidator _validator = new();

    [Fact]
    public async Task ValidQuery_ShouldNotHaveValidationErrors()
    {
        var query = new SearchAthletesQuery
        {
            SearchTerm = "test",
            Name = "John",
            Page = 1,
            PageSize = 20
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task SearchTermExceedsMaxLength_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery
        {
            SearchTerm = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public async Task PageSizeZero_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { PageSize = 0 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task PageSizeOver100_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { PageSize = 101 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task PageZero_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { Page = 0 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public async Task InvalidSortBy_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { SortBy = "invalidfield" };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task ValidSortBy_ShouldNotHaveValidationError()
    {
        var query = new SearchAthletesQuery { SortBy = "name" };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task NegativeMinAge_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { MinAge = -1 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.MinAge);
    }

    [Fact]
    public async Task MinAgeOver120_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { MinAge = 121 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.MinAge);
    }

    [Fact]
    public async Task NegativeExperience_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery { MinExperience = -1 };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.MinExperience);
    }

    [Fact]
    public async Task CreatedFromAfterCreatedTo_ShouldHaveValidationError()
    {
        var query = new SearchAthletesQuery
        {
            CreatedFrom = DateTime.UtcNow,
            CreatedTo = DateTime.UtcNow.AddDays(-1)
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.CreatedFrom);
    }
}
