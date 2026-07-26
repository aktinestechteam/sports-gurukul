using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class AdvancedSearchAcademiesValidatorTests
{
    private readonly AdvancedSearchAcademiesValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_NoErrors()
    {
        var query = new AdvancedSearchAcademiesQuery
        {
            SearchTerm = "cricket",
            Page = 1,
            PageSize = 20
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_PageLessThanOne_Fails()
    {
        var query = new AdvancedSearchAcademiesQuery
        {
            Page = 0,
            PageSize = 20
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be at least 1.");
    }

    [Fact]
    public async Task Validate_PageSizeExceedsMaximum_Fails()
    {
        var query = new AdvancedSearchAcademiesQuery
        {
            Page = 1,
            PageSize = 101
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }
}
