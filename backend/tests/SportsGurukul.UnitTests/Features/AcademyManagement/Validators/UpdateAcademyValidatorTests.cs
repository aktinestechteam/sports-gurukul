using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class UpdateAcademyValidatorTests
{
    private readonly UpdateAcademyValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Name = "Updated Academy",
            Email = "updated@test.com",
            Website = "https://example.com",
            LogoUrl = "https://example.com/logo.png"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAcademyId_HasError()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.Empty,
            Name = "Updated Academy"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AcademyId");
    }

    [Fact]
    public void Validate_InvalidEmail_HasError()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Email = "not-an-email"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_NameExceedsMaxLength_HasError()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Name = new string('A', 201)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_InvalidWebsite_HasError()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            Website = "not-a-url"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Website");
    }

    [Fact]
    public void Validate_InvalidLogoUrl_HasError()
    {
        var command = new UpdateAcademyCommand
        {
            AcademyId = Guid.NewGuid(),
            LogoUrl = "not-a-url"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LogoUrl");
    }
}
