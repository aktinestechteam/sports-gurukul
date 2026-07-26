using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;
using SportsGurukul.Application.Features.AcademyManagement.Validators;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class CreateFacilityValidatorTests
{
    private readonly CreateFacilityValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new CreateFacilityCommand
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "Basketball Court",
            FacilityType = AcademyFacilityType.Court,
            Capacity = 30
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new CreateFacilityCommand
        {
            AcademyId = Guid.Empty,
            FacilityName = "Basketball Court",
            FacilityType = AcademyFacilityType.Court
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_EmptyFacilityName_HasError()
    {
        var command = new CreateFacilityCommand
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "",
            FacilityType = AcademyFacilityType.Court
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FacilityName");
    }

    [Fact]
    public void Validate_CapacityLessThanOne_HasError()
    {
        var command = new CreateFacilityCommand
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "Basketball Court",
            FacilityType = AcademyFacilityType.Court,
            Capacity = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Capacity");
    }
}
