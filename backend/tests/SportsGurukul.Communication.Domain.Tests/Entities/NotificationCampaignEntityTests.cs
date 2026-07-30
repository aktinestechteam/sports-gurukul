using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationCampaignEntityTests
{
    [Fact]
    public void CreateCampaign_WithAllProperties_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var campaign = new NotificationCampaign
        {
            Id = id,
            Name = "Summer Sale 2026",
            Description = "Promotional campaign for summer sale",
            TemplateId = templateId,
            ChannelType = NotificationChannelType.Email,
            Status = NotificationStatus.Draft,
            ScheduledAt = now.AddDays(7),
            StartedAt = null,
            CompletedAt = null,
            TargetCriteria = "{\"age\":\"18-35\",\"location\":\"all\"}",
            TotalCount = 10000,
            SuccessCount = 0,
            FailureCount = 0,
            Metadata = "{\"campaign_type\":\"promotional\"}",
            CreatedAt = now
        };

        campaign.Id.Should().Be(id);
        campaign.Name.Should().Be("Summer Sale 2026");
        campaign.Description.Should().Be("Promotional campaign for summer sale");
        campaign.TemplateId.Should().Be(templateId);
        campaign.ChannelType.Should().Be(NotificationChannelType.Email);
        campaign.Status.Should().Be(NotificationStatus.Draft);
        campaign.ScheduledAt.Should().Be(now.AddDays(7));
        campaign.StartedAt.Should().BeNull();
        campaign.CompletedAt.Should().BeNull();
        campaign.TargetCriteria.Should().Be("{\"age\":\"18-35\",\"location\":\"all\"}");
        campaign.TotalCount.Should().Be(10000);
        campaign.SuccessCount.Should().Be(0);
        campaign.FailureCount.Should().Be(0);
        campaign.Metadata.Should().Be("{\"campaign_type\":\"promotional\"}");
        campaign.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void DefaultStatus_ShouldBeDraft()
    {
        var campaign = new NotificationCampaign();

        campaign.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public void Counts_ShouldBeZero_OnCreation()
    {
        var campaign = new NotificationCampaign();

        campaign.TotalCount.Should().Be(0);
        campaign.SuccessCount.Should().Be(0);
        campaign.FailureCount.Should().Be(0);
    }

    [Fact]
    public void ScheduledDate_ShouldBeAssignable()
    {
        var scheduledDate = new DateTime(2026, 12, 25, 10, 0, 0, DateTimeKind.Utc);

        var campaign = new NotificationCampaign { ScheduledAt = scheduledDate };

        campaign.ScheduledAt.Should().Be(scheduledDate);
        campaign.ScheduledAt.Value.Date.Should().Be(new DateOnly(2026, 12, 25).ToDateTime(TimeOnly.MinValue).Date);
    }

    [Fact]
    public void ScheduledDate_ShouldBeNull_WhenNotSet()
    {
        var campaign = new NotificationCampaign();

        campaign.ScheduledAt.Should().BeNull();
    }

    [Fact]
    public void CampaignProgress_ShouldTrackSuccessAndFailureCounts()
    {
        var campaign = new NotificationCampaign
        {
            TotalCount = 1000,
            SuccessCount = 750,
            FailureCount = 250
        };

        campaign.SuccessCount.Should().Be(750);
        campaign.FailureCount.Should().Be(250);
        (campaign.SuccessCount + campaign.FailureCount).Should().Be(1000);
    }

    [Fact]
    public void Campaign_WithNotifications_ShouldInitializeCollection()
    {
        var campaign = new NotificationCampaign();

        campaign.Notifications.Should().NotBeNull();
        campaign.Notifications.Should().BeEmpty();
        campaign.Notifications.Should().BeAssignableTo<ICollection<Notification>>();
    }

    [Fact]
    public void Campaign_WithTemplate_ShouldSetNavigationProperty()
    {
        var template = new NotificationTemplate { Id = Guid.NewGuid(), Name = "Campaign Template" };

        var campaign = new NotificationCampaign
        {
            TemplateId = template.Id,
            Template = template
        };

        campaign.Template.Should().NotBeNull();
        campaign.Template.Should().Be(template);
        campaign.Template.Name.Should().Be("Campaign Template");
    }

    [Fact]
    public void Description_ShouldBeNull_WhenNotSet()
    {
        var campaign = new NotificationCampaign();

        campaign.Description.Should().BeNull();
    }

    [Fact]
    public void StartedAt_CompletedAt_ShouldBeNull_ByDefault()
    {
        var campaign = new NotificationCampaign();

        campaign.StartedAt.Should().BeNull();
        campaign.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Status_ShouldTransitionThroughCampaignLifecycle()
    {
        var campaign = new NotificationCampaign();

        campaign.Status = NotificationStatus.Queued;
        campaign.Status.Should().Be(NotificationStatus.Queued);

        campaign.Status = NotificationStatus.Sending;
        campaign.Status.Should().Be(NotificationStatus.Sending);

        campaign.Status = NotificationStatus.Sent;
        campaign.Status.Should().Be(NotificationStatus.Sent);
    }
}
