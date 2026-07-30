using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class CreateCampaignCommandValidatorTests
{
    private readonly CreateCampaignCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveErrors()
    {
        var command = new CreateCampaignCommand(
            Name: "Test Campaign",
            Description: "A test campaign",
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenNameExceedsMaxLength_ShouldHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: new string('x', 201),
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenChannelTypeIsInvalid_ShouldHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Test",
            Description: null,
            TemplateId: null,
            ChannelType: (NotificationChannelType)99,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelType);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInPast_ShouldHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Test",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: DateTime.UtcNow.AddHours(-1),
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInFuture_ShouldNotHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Test",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: DateTime.UtcNow.AddDays(1),
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsNull_ShouldNotHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Test",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
