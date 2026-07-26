using FluentAssertions;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Validators;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Validators;

public class CreateAcademyValidatorTests
{
    private readonly CreateAcademyValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameExceedsMaxLength_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = new string('A', 201),
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_EmptyEmail_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "",
            Phone = "1234567890"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_InvalidEmail_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "not-an-email",
            Phone = "1234567890"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyPhone_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_PhoneExceedsMaxLength_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = new string('9', 51)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_DescriptionExceedsMaxLength_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            Description = new string('D', 2001)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_InvalidWebsite_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            Website = "not-a-url"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Website");
    }

    [Fact]
    public void Validate_GSTNumberExceedsMaxLength_HasError()
    {
        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            GSTNumber = new string('G', 51)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "GSTNumber");
    }
}
