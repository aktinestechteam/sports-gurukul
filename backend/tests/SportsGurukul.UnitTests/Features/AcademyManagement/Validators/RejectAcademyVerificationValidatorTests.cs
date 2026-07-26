using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class RejectAcademyVerificationValidatorTests
{
    private readonly RejectAcademyVerificationValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = Guid.NewGuid(),
            Remarks = "Documents are invalid"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = Guid.Empty,
            Remarks = "Documents are invalid"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_EmptyRemarks_HasError()
    {
        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = Guid.NewGuid(),
            Remarks = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Remarks");
    }

    [Fact]
    public void Validate_RemarksExceedsMaxLength_HasError()
    {
        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = Guid.NewGuid(),
            Remarks = new string('R', 1001)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Remarks");
    }
}
