using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetPagedAthletes;

namespace SportsGurukul.Application.Tests.Validators;

public class GetPagedAthletesValidatorTests
{
    private readonly GetPagedAthletesValidator _validator = new();

    [Fact]
    public async Task ValidQuery_ShouldNotHaveValidationErrors()
    {
        var query = new GetPagedAthletesQuery
        {
            Page = 1,
            PageSize = 20,
            SortBy = "name"
        };

        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task PageZero_ShouldHaveValidationError()
    {
        var query = new GetPagedAthletesQuery { Page = 0 };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public async Task PageSizeOver100_ShouldHaveValidationError()
    {
        var query = new GetPagedAthletesQuery { PageSize = 101 };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task InvalidSortBy_ShouldHaveValidationError()
    {
        var query = new GetPagedAthletesQuery { SortBy = "invalid" };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task NullSortBy_ShouldNotHaveValidationError()
    {
        var query = new GetPagedAthletesQuery { SortBy = null };
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }
}
