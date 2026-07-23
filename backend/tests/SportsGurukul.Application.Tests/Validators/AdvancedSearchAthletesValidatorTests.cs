using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;

namespace SportsGurukul.Application.Tests.Validators;

public class AdvancedSearchAthletesValidatorTests
{
    private readonly AdvancedSearchAthletesValidator _validator = new();

    [Fact]
    public async Task ValidQuery_ShouldNotHaveValidationErrors()
    {
        var query = new AdvancedSearchAthletesQuery
        {
            SearchTerm = "test",
            Name = "John",
            City = "Mumbai",
            SortBy = "name",
            Page = 1,
            PageSize = 20
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task SearchTermExceedsMaxLength_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery
        {
            SearchTerm = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public async Task PageSizeZero_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { PageSize = 0 };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task PageSizeOver100_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { PageSize = 101 };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task InvalidSortBy_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { SortBy = "invalidfield" };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task ValidSortByRanking_ShouldNotHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { SortBy = "ranking" };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task NegativeMinAge_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { MinAge = -1 };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.MinAge);
    }

    [Fact]
    public async Task EmptySportIdsList_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { SportIds = new List<Guid>() };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SportIds);
    }

    [Fact]
    public async Task TooManySportIds_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery { SportIds = Enumerable.Range(1, 21).Select(_ => Guid.NewGuid()).ToList() };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SportIds);
    }

    [Fact]
    public async Task CreatedFromAfterCreatedTo_ShouldHaveValidationError()
    {
        var query = new AdvancedSearchAthletesQuery
        {
            CreatedFrom = DateTime.UtcNow,
            CreatedTo = DateTime.UtcNow.AddDays(-1)
        };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.CreatedFrom);
    }
}
