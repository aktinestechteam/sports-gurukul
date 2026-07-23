using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;
using SportsGurukul.Application.Features.UserManagement.Validators;

namespace SportsGurukul.UnitTests.UserManagement;

public class SearchUsersValidatorTests
{
    private readonly SearchUsersValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidRequest()
    {
        var query = new SearchUsersQuery
        {
            SearchTerm = "test",
            Page = 1,
            PageSize = 20,
            SortBy = "name"
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_SearchTermExceeds200Characters()
    {
        var query = new SearchUsersQuery
        {
            SearchTerm = new string('a', 201)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Search term must not exceed 200 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_NameExceeds100Characters()
    {
        var query = new SearchUsersQuery
        {
            Name = new string('a', 101)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not exceed 100 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_EmailExceeds200Characters()
    {
        var query = new SearchUsersQuery
        {
            Email = new string('a', 201)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 200 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_MobileExceeds20Characters()
    {
        var query = new SearchUsersQuery
        {
            Mobile = new string('1', 21)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Mobile)
            .WithErrorMessage("Mobile must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageIsZero()
    {
        var query = new SearchUsersQuery { Page = 0 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be at least 1.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageIsNegative()
    {
        var query = new SearchUsersQuery { Page = -1 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageSizeIsZero()
    {
        var query = new SearchUsersQuery { PageSize = 0 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageSizeExceeds100()
    {
        var query = new SearchUsersQuery { PageSize = 101 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_PageSizeIs100()
    {
        var query = new SearchUsersQuery { PageSize = 100 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("email")]
    [InlineData("role")]
    [InlineData("createddate")]
    [InlineData("updateddate")]
    public async Task Validate_Should_Pass_When_SortByIsValid(string sortBy)
    {
        var query = new SearchUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalidfield")]
    [InlineData("DateCreated")]
    public async Task Validate_Should_Fail_When_SortByIsInvalid(string sortBy)
    {
        var query = new SearchUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("NAME")]
    [InlineData("Email")]
    public async Task Validate_Should_Pass_When_SortByIsCaseInsensitive(string sortBy)
    {
        var query = new SearchUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_SortByIsNull()
    {
        var query = new SearchUsersQuery { SortBy = null };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_CreatedFromAfterCreatedTo()
    {
        var query = new SearchUsersQuery
        {
            CreatedFrom = DateTime.UtcNow,
            CreatedTo = DateTime.UtcNow.AddDays(-1)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.CreatedFrom)
            .WithErrorMessage("CreatedFrom must be before or equal to CreatedTo.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UpdatedFromAfterUpdatedTo()
    {
        var query = new SearchUsersQuery
        {
            UpdatedFrom = DateTime.UtcNow,
            UpdatedTo = DateTime.UtcNow.AddDays(-1)
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.UpdatedFrom)
            .WithErrorMessage("UpdatedFrom must be before or equal to UpdatedTo.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_DateRangesAreValid()
    {
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;
        var query = new SearchUsersQuery
        {
            CreatedFrom = from,
            CreatedTo = to,
            UpdatedFrom = from,
            UpdatedTo = to
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
