using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class AdditionalNotificationValidatorTests
{
    private readonly CreateNotificationCommandValidator _createValidator = new();
    private readonly ScheduleNotificationCommandValidator _scheduleValidator = new();
    private readonly RetryNotificationCommandValidator _retryValidator = new();
    private readonly CreateCampaignCommandValidator _campaignValidator = new();
    private readonly UpdatePreferenceCommandValidator _preferenceValidator = new();
    private readonly CreateTemplateCommandValidator _templateValidator = new();

    [Fact]
    public void Recipients_WhenEmailFormatIsValid_ShouldNotHaveError()
    {
        var command = ValidCreateCommand(recipients: new List<CreateRecipientRequest>
        {
            new(Guid.NewGuid(), "Email", "user@example.com", "User")
        });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Recipients_WhenDestinationAddressIsEmpty_ShouldHaveError()
    {
        var command = ValidCreateCommand(recipients: new List<CreateRecipientRequest>
        {
            new(Guid.NewGuid(), "Email", "", "User")
        });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Recipients[0].DestinationAddress");
    }

    [Fact]
    public void Recipients_WhenListIsEmpty_ShouldHaveError()
    {
        var command = ValidCreateCommand(recipients: new List<CreateRecipientRequest>());

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Recipients);
    }

    [Fact]
    public void Recipients_WhenMultipleRecipientsAllValid_ShouldNotHaveError()
    {
        var command = ValidCreateCommand(recipients: new List<CreateRecipientRequest>
        {
            new(Guid.NewGuid(), "Email", "user1@example.com", "User1"),
            new(Guid.NewGuid(), "Email", "user2@example.com", "User2"),
            new(Guid.NewGuid(), "Email", "user3@example.com", "User3")
        });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ChannelSelection_WhenChannelIsValidEnum_ShouldNotHaveError()
    {
        var command = ValidCreateCommand();

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ChannelSelection_WhenChannelIdIsEmpty_ShouldHaveError()
    {
        var command = ValidCreateCommand() with { ChannelId = Guid.Empty };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelId);
    }

    [Fact]
    public void Priority_WhenValidEnumValues_ShouldNotHaveError()
    {
        foreach (var priority in Enum.GetValues<NotificationPriority>())
        {
            var command = ValidCreateCommand() with { Priority = priority };

            var result = _createValidator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }
    }

    [Fact]
    public void Priority_WhenInvalidEnumValue_ShouldHaveError()
    {
        var command = ValidCreateCommand() with { Priority = (NotificationPriority)(-1) };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Scheduling_FutureDate_ShouldNotHaveError()
    {
        var command = ValidCreateCommand() with { ScheduledAt = DateTime.UtcNow.AddDays(7) };

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Scheduling_PastDate_ShouldHaveError()
    {
        var command = ValidCreateCommand() with { ScheduledAt = DateTime.UtcNow.AddDays(-1) };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Scheduling_ScheduleCommandFuture_ShouldNotHaveError()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _scheduleValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Scheduling_ScheduleCommandPast_ShouldHaveError()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        var result = _scheduleValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void CampaignDates_FutureSchedule_ShouldNotHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Campaign",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: DateTime.UtcNow.AddDays(1),
            TargetCriteria: null,
            Metadata: null
        );

        var result = _campaignValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void CampaignDates_PastSchedule_ShouldHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Campaign",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: DateTime.UtcNow.AddDays(-1),
            TargetCriteria: null,
            Metadata: null
        );

        var result = _campaignValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void CampaignDates_NoSchedule_ShouldNotHaveError()
    {
        var command = new CreateCampaignCommand(
            Name: "Campaign",
            Description: null,
            TemplateId: null,
            ChannelType: NotificationChannelType.Email,
            ScheduledAt: null,
            TargetCriteria: null,
            Metadata: null
        );

        var result = _campaignValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Retry_ValidCommand_ShouldNotHaveError()
    {
        var command = new RetryNotificationCommand(Guid.NewGuid());

        var result = _retryValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Retry_EmptyId_ShouldHaveError()
    {
        var command = new RetryNotificationCommand(Guid.Empty);

        var result = _retryValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void QuietHours_ValidPreference_ShouldNotHaveError()
    {
        var command = new UpdatePreferenceCommand(
            Guid.NewGuid(),
            NotificationChannelType.Email,
            true,
            new TimeOnly(22, 0),
            new TimeOnly(8, 0),
            10
        );

        var result = _preferenceValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void QuietHours_InvalidChannelType_ShouldHaveError()
    {
        var command = new UpdatePreferenceCommand(
            Guid.NewGuid(),
            (NotificationChannelType)99,
            true,
            null,
            null,
            null
        );

        var result = _preferenceValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelType);
    }

    [Fact]
    public void Template_ValidWithVariables_ShouldNotHaveError()
    {
        var command = new CreateTemplateCommand(
            "Template",
            null,
            NotificationChannelType.Email,
            "Hello {{name}}",
            "Welcome {{name}}!",
            new List<CreateTemplateVariableRequest>
            {
                new("name", "User name", true, null, "string")
            }
        );

        var result = _templateValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Template_EmptyName_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            "",
            null,
            NotificationChannelType.Email,
            "Subject",
            "Body",
            null
        );

        var result = _templateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Template_EmptyBody_ShouldHaveError()
    {
        var command = new CreateTemplateCommand(
            "Template",
            null,
            NotificationChannelType.Email,
            "Subject",
            "",
            null
        );

        var result = _templateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BodyTemplate);
    }

    [Fact]
    public void DuplicateCheck_WithDifferentExternalIds_ShouldNotConflict()
    {
        var command1 = ValidCreateCommand() with { ExternalId = "ext-1" };
        var command2 = ValidCreateCommand() with { ExternalId = "ext-2" };

        var result1 = _createValidator.TestValidate(command1);
        var result2 = _createValidator.TestValidate(command2);

        result1.ShouldNotHaveAnyValidationErrors();
        result2.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateNotificationCommand ValidCreateCommand(
        List<CreateRecipientRequest>? recipients = null) =>
        new(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: Guid.NewGuid(),
            Priority: NotificationPriority.Normal,
            Subject: "Test Subject",
            Body: "Test Body",
            SenderId: "sender",
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: recipients ?? new List<CreateRecipientRequest>
            {
                new(Guid.NewGuid(), "Email", "test@example.com", "Test")
            },
            Attachments: null
        );
}
