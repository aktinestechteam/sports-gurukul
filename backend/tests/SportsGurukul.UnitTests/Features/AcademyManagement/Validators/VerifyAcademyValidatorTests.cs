using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.VerifyAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class VerifyAcademyValidatorTests
{
    private readonly VerifyAcademyValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new VerifyAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Remarks = "All documents verified"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new VerifyAcademyCommand
        {
            AcademyId = Guid.Empty,
            Remarks = "Verified"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_RemarksExceedsMaxLength_HasError()
    {
        var command = new VerifyAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Remarks = new string('R', 1001)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Remarks");
    }
}
