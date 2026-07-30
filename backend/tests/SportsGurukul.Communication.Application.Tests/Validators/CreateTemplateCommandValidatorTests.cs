using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class CreateTemplateCommandValidatorTests
{
    private readonly CreateTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveErrors()
    {
        var command = new CreateTemplateCommand(
            Name: "Welcome Template",
            Description: "Template for welcome emails",
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Welcome {{name}}!",
            BodyTemplate: "Hello {{name}}, welcome to our platform.",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "",
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Subject",
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenNameExceedsMaxLength_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: new string('x', 201),
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Subject",
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenChannelTypeIsInvalid_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "Test",
            Description: null,
            ChannelType: (NotificationChannelType)99,
            SubjectTemplate: "Subject",
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelType);
    }

    [Fact]
    public void Validate_WhenSubjectTemplateIsEmpty_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "Test",
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "",
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SubjectTemplate);
    }

    [Fact]
    public void Validate_WhenSubjectTemplateExceedsMaxLength_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "Test",
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: new string('x', 501),
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SubjectTemplate);
    }

    [Fact]
    public void Validate_WhenBodyTemplateIsEmpty_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "Test",
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Subject",
            BodyTemplate: "",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BodyTemplate);
    }

    [Fact]
    public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            Name: "Test",
            Description: new string('x', 1001),
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Subject",
            BodyTemplate: "Body",
            Variables: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
