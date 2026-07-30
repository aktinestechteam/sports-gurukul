using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class CreateNotificationCommandValidatorTests
{
    private readonly CreateNotificationCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveErrors()
    {
        var command = ValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenChannelIdIsEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { ChannelId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelId);
    }

    [Fact]
    public void Validate_WhenSubjectIsEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { Subject = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Fact]
    public void Validate_WhenSubjectExceedsMaxLength_ShouldHaveError()
    {
        var command = ValidCommand() with { Subject = new string('x', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Fact]
    public void Validate_WhenBodyIsEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { Body = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Body);
    }

    [Fact]
    public void Validate_WhenPriorityIsInvalid_ShouldHaveError()
    {
        var command = ValidCommand() with { Priority = (NotificationPriority)99 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Validate_WhenRecipientsIsEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { Recipients = new List<CreateRecipientRequest>() };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Recipients);
    }

    [Fact]
    public void Validate_WhenRecipientDestinationAddressIsEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with
        {
            Recipients = new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "", RecipientName: null)
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Recipients[0].DestinationAddress");
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInPast_ShouldHaveError()
    {
        var command = ValidCommand() with { ScheduledAt = DateTime.UtcNow.AddHours(-1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInFuture_ShouldNotHaveError()
    {
        var command = ValidCommand() with { ScheduledAt = DateTime.UtcNow.AddDays(1) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsNull_ShouldNotHaveError()
    {
        var command = ValidCommand() with { ScheduledAt = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenExternalIdExceedsMaxLength_ShouldHaveError()
    {
        var command = ValidCommand() with { ExternalId = new string('x', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExternalId);
    }

    [Fact]
    public void Validate_WhenMetadataExceedsMaxLength_ShouldHaveError()
    {
        var command = ValidCommand() with { Metadata = new string('x', 4001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Metadata);
    }

    private static CreateNotificationCommand ValidCommand() =>
        new(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: Guid.NewGuid(),
            Priority: NotificationPriority.High,
            Subject: "Test Subject",
            Body: "Test Body",
            SenderId: "sender-1",
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: "ext-1",
            Metadata: "{}",
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "user@example.com", RecipientName: "User")
            },
            Attachments: null
        );
}
