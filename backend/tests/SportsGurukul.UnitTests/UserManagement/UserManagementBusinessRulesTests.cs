using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class UserManagementBusinessRulesTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void ProfileCompletion_Should_Return0Percent_When_AllFieldsEmpty()
    {
        var dto = new UserProfileDto
        {
            Bio = null,
            DateOfBirth = null,
            Gender = Gender.PreferNotToSay,
            Height = null,
            Weight = null,
            PreferredSport = null,
            ExperienceLevel = null,
            ContactInformation = null
        };

        var percentage = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dto);

        percentage.Should().Be(0);
    }

    [Fact]
    public void ProfileCompletion_Should_Return100Percent_When_AllFieldsFilled()
    {
        var dto = new UserProfileDto
        {
            Bio = "Athlete bio",
            DateOfBirth = new DateTime(2000, 1, 1),
            Gender = Gender.Male,
            Height = "180cm",
            Weight = "75kg",
            PreferredSport = "Cricket",
            ExperienceLevel = "Intermediate",
            ContactInformation = new ContactDto { PrimaryPhoneNumber = "1234567890" }
        };

        var percentage = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dto);

        percentage.Should().Be(100);
    }

    [Fact]
    public void ProfileCompletion_Should_Return12Percent_When_OnlyBioFilled()
    {
        var dto = new UserProfileDto
        {
            Bio = "Athlete",
            DateOfBirth = null,
            Gender = Gender.PreferNotToSay,
            Height = null,
            Weight = null,
            PreferredSport = null,
            ExperienceLevel = null,
            ContactInformation = null
        };

        var percentage = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dto);

        percentage.Should().Be(12);
    }

    [Fact]
    public void ProfileCompletion_Should_CountGender_When_NotPreferNotToSay()
    {
        var dtoWithoutGender = new UserProfileDto { Gender = Gender.PreferNotToSay };
        var dtoWithGender = new UserProfileDto { Gender = Gender.Male };

        var pctWithout = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dtoWithoutGender);
        var pctWith = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dtoWithGender);

        pctWith.Should().BeGreaterThan(pctWithout);
    }

    [Fact]
    public void ProfileCompletion_Should_CountPhone_When_ContactInformationHasPhone()
    {
        var dtoWithoutPhone = new UserProfileDto { ContactInformation = null };
        var dtoWithPhone = new UserProfileDto
        {
            ContactInformation = new ContactDto { PrimaryPhoneNumber = "1234567890" }
        };

        var pctWithout = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dtoWithoutPhone);
        var pctWith = CreateUserProfileCommandHandler.CalculateCompletionPercentage(dtoWithPhone);

        pctWith.Should().BeGreaterThan(pctWithout);
    }

    [Fact]
    public void PaginationResponse_Should_HaveCorrectHasPrevious()
    {
        var response = new PaginationResponse<UserListDto>
        {
            CurrentPage = 2,
            TotalPages = 5,
            PageSize = 10
        };

        response.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void PaginationResponse_Should_HaveNoPrevious_When_OnFirstPage()
    {
        var response = new PaginationResponse<UserListDto>
        {
            CurrentPage = 1,
            TotalPages = 5,
            PageSize = 10
        };

        response.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void PaginationResponse_Should_HaveCorrectHasNext()
    {
        var response = new PaginationResponse<UserListDto>
        {
            CurrentPage = 2,
            TotalPages = 5,
            PageSize = 10
        };

        response.HasNext.Should().BeTrue();
    }

    [Fact]
    public void PaginationResponse_Should_HaveNoNext_When_OnLastPage()
    {
        var response = new PaginationResponse<UserListDto>
        {
            CurrentPage = 5,
            TotalPages = 5,
            PageSize = 10
        };

        response.HasNext.Should().BeFalse();
    }

    [Fact]
    public void PaginationRequest_Should_CapPageSizeAtMax_When_Exceeds100()
    {
        var request = new PaginationRequest { PageSize = 200 };

        request.PageSize.Should().Be(100);
    }

    [Fact]
    public void PaginationRequest_Should_AllowPageSize_When_Under100()
    {
        var request = new PaginationRequest { PageSize = 50 };

        request.PageSize.Should().Be(50);
    }

    [Fact]
    public void Result_Should_BeSuccess_When_CreatedWithSuccess()
    {
        var result = Result<UserProfileDto>.Success(new UserProfileDto());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Result_Should_BeFailure_When_CreatedWithError()
    {
        var result = Result<UserProfileDto>.Failure("Something went wrong");

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Something went wrong");
    }

    [Fact]
    public void Result_Should_BeFailure_When_CreatedWithMultipleErrors()
    {
        var errors = new List<string> { "Error 1", "Error 2" };
        var result = Result<UserProfileDto>.Failure(errors);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Error.Should().Be("Error 1");
    }
}
