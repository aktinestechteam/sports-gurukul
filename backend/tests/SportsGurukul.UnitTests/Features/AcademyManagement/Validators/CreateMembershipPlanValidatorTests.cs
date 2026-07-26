using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class CreateMembershipPlanValidatorTests
{
    private readonly CreateMembershipPlanValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new CreateMembershipPlanCommand
        {
            AcademyId = Guid.NewGuid(),
            MembershipName = "Premium Plan",
            Price = 99.99m,
            Duration = 12
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new CreateMembershipPlanCommand
        {
            AcademyId = Guid.Empty,
            MembershipName = "Premium Plan",
            Price = 99.99m,
            Duration = 12
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateMembershipPlanCommand
        {
            AcademyId = Guid.NewGuid(),
            MembershipName = "",
            Price = 99.99m,
            Duration = 12
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipName");
    }

    [Fact]
    public void Validate_NegativePrice_HasError()
    {
        var command = new CreateMembershipPlanCommand
        {
            AcademyId = Guid.NewGuid(),
            MembershipName = "Premium Plan",
            Price = -1m,
            Duration = 12
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_DurationLessThanOne_HasError()
    {
        var command = new CreateMembershipPlanCommand
        {
            AcademyId = Guid.NewGuid(),
            MembershipName = "Premium Plan",
            Price = 99.99m,
            Duration = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Duration");
    }
}
