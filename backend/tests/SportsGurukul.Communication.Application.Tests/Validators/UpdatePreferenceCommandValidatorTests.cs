using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class UpdatePreferenceCommandValidatorTests
{
    private readonly UpdatePreferenceCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveErrors()
    {
        var command = new UpdatePreferenceCommand(
            UserId: Guid.NewGuid(),
            ChannelType: NotificationChannelType.Email,
            IsEnabled: true,
            QuietHoursStart: new TimeOnly(22, 0),
            QuietHoursEnd: new TimeOnly(8, 0),
            MaxPerDay: 10
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError()
    {
        var command = new UpdatePreferenceCommand(
            UserId: Guid.Empty,
            ChannelType: NotificationChannelType.Email,
            IsEnabled: true,
            QuietHoursStart: null,
            QuietHoursEnd: null,
            MaxPerDay: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenChannelTypeIsInvalid_ShouldHaveError()
    {
        var command = new UpdatePreferenceCommand(
            UserId: Guid.NewGuid(),
            ChannelType: (NotificationChannelType)99,
            IsEnabled: true,
            QuietHoursStart: null,
            QuietHoursEnd: null,
            MaxPerDay: null
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ChannelType);
    }

    [Fact]
    public void Validate_AllValidChannelTypes_ShouldNotHaveErrors()
    {
        foreach (var channelType in Enum.GetValues<NotificationChannelType>())
        {
            var command = new UpdatePreferenceCommand(
                UserId: Guid.NewGuid(),
                ChannelType: channelType,
                IsEnabled: true,
                QuietHoursStart: null,
                QuietHoursEnd: null,
                MaxPerDay: null
            );

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ChannelType);
        }
    }
}
