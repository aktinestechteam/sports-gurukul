using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class DeleteAndRestoreAcademyValidatorTests
{
    private readonly DeleteAcademyValidator _deleteValidator = new();
    private readonly RestoreAcademyValidator _restoreValidator = new();

    [Fact]
    public void DeleteAcademy_ValidRequest_NoErrors()
    {
        var command = new DeleteAcademyCommand
        {
            AcademyId = Guid.NewGuid()
        };

        var result = _deleteValidator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteAcademy_EmptyAcademyId_HasError()
    {
        var command = new DeleteAcademyCommand
        {
            AcademyId = Guid.Empty
        };

        var result = _deleteValidator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void RestoreAcademy_ValidRequest_NoErrors()
    {
        var command = new RestoreAcademyCommand
        {
            AcademyId = Guid.NewGuid()
        };

        var result = _restoreValidator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RestoreAcademy_EmptyAcademyId_HasError()
    {
        var command = new RestoreAcademyCommand
        {
            AcademyId = Guid.Empty
        };

        var result = _restoreValidator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }
}
