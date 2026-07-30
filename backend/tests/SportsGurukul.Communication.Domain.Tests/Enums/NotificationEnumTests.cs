using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Enums;

public class NotificationEnumTests
{
    public static TheoryData<NotificationChannelType, int, string> NotificationChannelTypeData =>
        new()
        {
            { NotificationChannelType.Email, 0, "Email" },
            { NotificationChannelType.SMS, 1, "SMS" },
            { NotificationChannelType.WhatsApp, 2, "WhatsApp" },
            { NotificationChannelType.PushNotification, 3, "PushNotification" },
            { NotificationChannelType.InAppNotification, 4, "InAppNotification" },
            { NotificationChannelType.Webhook, 5, "Webhook" }
        };

    public static TheoryData<NotificationPriority, int, string> NotificationPriorityData =>
        new()
        {
            { NotificationPriority.Low, 0, "Low" },
            { NotificationPriority.Normal, 1, "Normal" },
            { NotificationPriority.High, 2, "High" },
            { NotificationPriority.Critical, 3, "Critical" }
        };

    public static TheoryData<NotificationStatus, int, string> NotificationStatusData =>
        new()
        {
            { NotificationStatus.Draft, 0, "Draft" },
            { NotificationStatus.Queued, 1, "Queued" },
            { NotificationStatus.Scheduled, 2, "Scheduled" },
            { NotificationStatus.Sending, 3, "Sending" },
            { NotificationStatus.Sent, 4, "Sent" },
            { NotificationStatus.Delivered, 5, "Delivered" },
            { NotificationStatus.Read, 6, "Read" },
            { NotificationStatus.Failed, 7, "Failed" },
            { NotificationStatus.Cancelled, 8, "Cancelled" },
            { NotificationStatus.Expired, 9, "Expired" }
        };

    [Theory]
    [MemberData(nameof(NotificationChannelTypeData))]
    public void NotificationChannelType_ShouldHaveAllValues(NotificationChannelType channelType, int expectedValue, string expectedName)
    {
        ((int)channelType).Should().Be(expectedValue);
        channelType.ToString().Should().Be(expectedName);
    }

    [Fact]
    public void NotificationChannelType_ShouldHaveSixValues()
    {
        var values = Enum.GetValues<NotificationChannelType>();

        values.Should().HaveCount(6);
        values.Should().Contain(NotificationChannelType.Email);
        values.Should().Contain(NotificationChannelType.SMS);
        values.Should().Contain(NotificationChannelType.WhatsApp);
        values.Should().Contain(NotificationChannelType.PushNotification);
        values.Should().Contain(NotificationChannelType.InAppNotification);
        values.Should().Contain(NotificationChannelType.Webhook);
    }

    [Fact]
    public void NotificationChannelType_DefaultValue_ShouldBeEmail()
    {
        NotificationChannelType defaultChannel = default;

        defaultChannel.Should().Be(NotificationChannelType.Email);
    }

    [Theory]
    [MemberData(nameof(NotificationPriorityData))]
    public void NotificationPriority_ShouldHaveAllValues(NotificationPriority priority, int expectedValue, string expectedName)
    {
        ((int)priority).Should().Be(expectedValue);
        priority.ToString().Should().Be(expectedName);
    }

    [Fact]
    public void NotificationPriority_ShouldHaveFourValues()
    {
        var values = Enum.GetValues<NotificationPriority>();

        values.Should().HaveCount(4);
        values.Should().Contain(NotificationPriority.Low);
        values.Should().Contain(NotificationPriority.Normal);
        values.Should().Contain(NotificationPriority.High);
        values.Should().Contain(NotificationPriority.Critical);
    }

    [Fact]
    public void NotificationPriority_DefaultValue_ShouldBeLow()
    {
        NotificationPriority defaultPriority = default;

        defaultPriority.Should().Be(NotificationPriority.Low);
    }

    [Theory]
    [MemberData(nameof(NotificationStatusData))]
    public void NotificationStatus_ShouldHaveAllValues(NotificationStatus status, int expectedValue, string expectedName)
    {
        ((int)status).Should().Be(expectedValue);
        status.ToString().Should().Be(expectedName);
    }

    [Fact]
    public void NotificationStatus_ShouldHaveTenValues()
    {
        var values = Enum.GetValues<NotificationStatus>();

        values.Should().HaveCount(10);
        values.Should().Contain(NotificationStatus.Draft);
        values.Should().Contain(NotificationStatus.Queued);
        values.Should().Contain(NotificationStatus.Scheduled);
        values.Should().Contain(NotificationStatus.Sending);
        values.Should().Contain(NotificationStatus.Sent);
        values.Should().Contain(NotificationStatus.Delivered);
        values.Should().Contain(NotificationStatus.Read);
        values.Should().Contain(NotificationStatus.Failed);
        values.Should().Contain(NotificationStatus.Cancelled);
        values.Should().Contain(NotificationStatus.Expired);
    }

    [Fact]
    public void NotificationStatus_DefaultValue_ShouldBeDraft()
    {
        NotificationStatus defaultStatus = default;

        defaultStatus.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public void NotificationStatus_Ordering_ShouldBeCorrect()
    {
        var allStatuses = Enum.GetValues<NotificationStatus>();

        allStatuses[0].Should().Be(NotificationStatus.Draft);
        allStatuses[1].Should().Be(NotificationStatus.Queued);
        allStatuses[2].Should().Be(NotificationStatus.Scheduled);
        allStatuses[3].Should().Be(NotificationStatus.Sending);
        allStatuses[4].Should().Be(NotificationStatus.Sent);
        allStatuses[5].Should().Be(NotificationStatus.Delivered);
        allStatuses[6].Should().Be(NotificationStatus.Read);
        allStatuses[7].Should().Be(NotificationStatus.Failed);
        allStatuses[8].Should().Be(NotificationStatus.Cancelled);
        allStatuses[9].Should().Be(NotificationStatus.Expired);
    }

    [Theory]
    [InlineData("Email", NotificationChannelType.Email)]
    [InlineData("SMS", NotificationChannelType.SMS)]
    [InlineData("WhatsApp", NotificationChannelType.WhatsApp)]
    [InlineData("PushNotification", NotificationChannelType.PushNotification)]
    [InlineData("InAppNotification", NotificationChannelType.InAppNotification)]
    [InlineData("Webhook", NotificationChannelType.Webhook)]
    public void ParseNotificationChannelType_FromString_ShouldReturnCorrectEnum(string value, NotificationChannelType expected)
    {
        var result = Enum.Parse<NotificationChannelType>(value);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Low", NotificationPriority.Low)]
    [InlineData("Normal", NotificationPriority.Normal)]
    [InlineData("High", NotificationPriority.High)]
    [InlineData("Critical", NotificationPriority.Critical)]
    public void ParseNotificationPriority_FromString_ShouldReturnCorrectEnum(string value, NotificationPriority expected)
    {
        var result = Enum.Parse<NotificationPriority>(value);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Draft", NotificationStatus.Draft)]
    [InlineData("Queued", NotificationStatus.Queued)]
    [InlineData("Scheduled", NotificationStatus.Scheduled)]
    [InlineData("Sending", NotificationStatus.Sending)]
    [InlineData("Sent", NotificationStatus.Sent)]
    [InlineData("Delivered", NotificationStatus.Delivered)]
    [InlineData("Read", NotificationStatus.Read)]
    [InlineData("Failed", NotificationStatus.Failed)]
    [InlineData("Cancelled", NotificationStatus.Cancelled)]
    [InlineData("Expired", NotificationStatus.Expired)]
    public void ParseNotificationStatus_FromString_ShouldReturnCorrectEnum(string value, NotificationStatus expected)
    {
        var result = Enum.Parse<NotificationStatus>(value);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("email", NotificationChannelType.Email)]
    [InlineData("sMS", NotificationChannelType.SMS)]
    [InlineData("WHATSAPP", NotificationChannelType.WhatsApp)]
    public void ParseNotificationChannelType_FromString_ShouldBeCaseInsensitive(string value, NotificationChannelType expected)
    {
        var result = Enum.Parse<NotificationChannelType>(value, ignoreCase: true);

        result.Should().Be(expected);
    }

    [Fact]
    public void TryParseNotificationStatus_InvalidString_ShouldReturnFalse()
    {
        var result = Enum.TryParse<NotificationStatus>("InvalidStatus", out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void TryParseNotificationChannelType_InvalidString_ShouldReturnFalse()
    {
        var result = Enum.TryParse<NotificationChannelType>("Invalid", out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void TryParseNotificationPriority_InvalidString_ShouldReturnFalse()
    {
        var result = Enum.TryParse<NotificationPriority>("Invalid", out _);

        result.Should().BeFalse();
    }
}
