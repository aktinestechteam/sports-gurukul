using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateBranch;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class CreateBranchValidatorTests
{
    private readonly CreateBranchValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Latitude = 19.0760m,
            Longitude = 72.8777m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.Empty,
            BranchName = "Main Branch"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_EmptyBranchName_HasError()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.NewGuid(),
            BranchName = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BranchName");
    }

    [Fact]
    public void Validate_BranchNameExceedsMaxLength_HasError()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.NewGuid(),
            BranchName = new string('B', 201)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BranchName");
    }

    [Fact]
    public void Validate_LatitudeOutOfRange_HasError()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Latitude = 91m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Latitude");
    }

    [Fact]
    public void Validate_LongitudeOutOfRange_HasError()
    {
        var command = new CreateBranchCommand
        {
            AcademyId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Longitude = 181m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Longitude");
    }
}
