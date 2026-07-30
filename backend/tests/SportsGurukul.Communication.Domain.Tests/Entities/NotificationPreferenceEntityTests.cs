using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationPreferenceEntityTests
{
    [Fact]
    public void CreatePreference_ForUser_ShouldSetPropertiesCorrectly()
    {
        var userId = Guid.NewGuid();

        var preference = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ChannelType = NotificationChannelType.Email,
            IsEnabled = true,
            QuietHoursStart = new TimeOnly(22, 0),
            QuietHoursEnd = new TimeOnly(8, 0),
            MaxPerDay = 10,
            CreatedAt = DateTime.UtcNow
        };

        preference.UserId.Should().Be(userId);
        preference.ChannelType.Should().Be(NotificationChannelType.Email);
        preference.IsEnabled.Should().BeTrue();
        preference.QuietHoursStart.Should().Be(new TimeOnly(22, 0));
        preference.QuietHoursEnd.Should().Be(new TimeOnly(8, 0));
        preference.MaxPerDay.Should().Be(10);
    }

    [Fact]
    public void IsEnabled_ShouldDefaultToTrue()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        preference.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportEmail()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        preference.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportSMS()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.SMS
        };

        preference.ChannelType.Should().Be(NotificationChannelType.SMS);
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportWhatsApp()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.WhatsApp
        };

        preference.ChannelType.Should().Be(NotificationChannelType.WhatsApp);
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportPushNotification()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.PushNotification
        };

        preference.ChannelType.Should().Be(NotificationChannelType.PushNotification);
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportInAppNotification()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.InAppNotification
        };

        preference.ChannelType.Should().Be(NotificationChannelType.InAppNotification);
    }

    [Fact]
    public void ChannelPreferences_ShouldSupportWebhook()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Webhook
        };

        preference.ChannelType.Should().Be(NotificationChannelType.Webhook);
    }

    [Fact]
    public void QuietHoursSettings_ShouldBeNull_WhenNotSet()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        preference.QuietHoursStart.Should().BeNull();
        preference.QuietHoursEnd.Should().BeNull();
    }

    [Fact]
    public void QuietHoursSettings_ShouldStoreStartAndEnd()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.SMS,
            QuietHoursStart = new TimeOnly(21, 0),
            QuietHoursEnd = new TimeOnly(7, 0)
        };

        preference.QuietHoursStart.Should().Be(new TimeOnly(21, 0));
        preference.QuietHoursEnd.Should().Be(new TimeOnly(7, 0));
    }

    [Fact]
    public void MaxPerDay_ShouldStoreDailyLimit()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            MaxPerDay = 5
        };

        preference.MaxPerDay.Should().Be(5);
    }

    [Fact]
    public void MaxPerDay_ShouldBeNull_WhenNotSet()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        preference.MaxPerDay.Should().BeNull();
    }

    [Fact]
    public void DisableChannel_ShouldSetIsEnabledToFalse()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            IsEnabled = true
        };

        preference.IsEnabled = false;

        preference.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void User_ShouldBeNull_WhenNotSet()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        preference.User.Should().BeNull();
    }
}
