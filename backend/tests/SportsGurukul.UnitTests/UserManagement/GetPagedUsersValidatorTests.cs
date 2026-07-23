using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Queries.GetPagedUsers;
using SportsGurukul.Application.Features.UserManagement.Validators;

namespace SportsGurukul.UnitTests.UserManagement;

public class GetPagedUsersValidatorTests
{
    private readonly GetPagedUsersValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidRequest()
    {
        var query = new GetPagedUsersQuery
        {
            Page = 1,
            PageSize = 20,
            SortBy = "name"
        };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageIsZero()
    {
        var query = new GetPagedUsersQuery { Page = 0 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be at least 1.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageIsNegative()
    {
        var query = new GetPagedUsersQuery { Page = -5 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageSizeIsZero()
    {
        var query = new GetPagedUsersQuery { PageSize = 0 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PageSizeExceeds100()
    {
        var query = new GetPagedUsersQuery { PageSize = 101 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_PageSizeIs1()
    {
        var query = new GetPagedUsersQuery { PageSize = 1 };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_PageSizeIs100()
    {
        var query = new GetPagedUsersQuery { PageSize = 100 };

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
        var query = new GetPagedUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalidfield")]
    [InlineData("DateCreated")]
    public async Task Validate_Should_Fail_When_SortByIsInvalid(string sortBy)
    {
        var query = new GetPagedUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("NAME")]
    [InlineData("Email")]
    public async Task Validate_Should_Pass_When_SortByIsCaseInsensitive(string sortBy)
    {
        var query = new GetPagedUsersQuery { SortBy = sortBy };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_SortByIsNull()
    {
        var query = new GetPagedUsersQuery { SortBy = null };

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }
}
