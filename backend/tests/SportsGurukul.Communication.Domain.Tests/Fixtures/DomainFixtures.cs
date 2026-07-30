using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Fixtures;

public class NotificationBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _channelId = Guid.NewGuid();
    private NotificationPriority _priority = NotificationPriority.Normal;
    private NotificationStatus _status = NotificationStatus.Draft;
    private string _subject = "Test Subject";
    private string _body = "Test Body";
    private Guid? _templateId;
    private Guid? _providerId;
    private Guid? _campaignId;
    private Guid? _batchId;
    private List<NotificationRecipient> _recipients = new();
    private List<NotificationAttachment> _attachments = new();
    private NotificationTemplate? _template;

    public NotificationBuilder WithId(Guid id) { _id = id; return this; }
    public NotificationBuilder WithChannelId(Guid channelId) { _channelId = channelId; return this; }
    public NotificationBuilder WithPriority(NotificationPriority priority) { _priority = priority; return this; }
    public NotificationBuilder WithStatus(NotificationStatus status) { _status = status; return this; }
    public NotificationBuilder WithSubject(string subject) { _subject = subject; return this; }
    public NotificationBuilder WithBody(string body) { _body = body; return this; }
    public NotificationBuilder WithTemplate(Guid templateId) { _templateId = templateId; return this; }
    public NotificationBuilder WithProvider(Guid providerId) { _providerId = providerId; return this; }
    public NotificationBuilder WithCampaign(Guid campaignId) { _campaignId = campaignId; return this; }
    public NotificationBuilder WithBatch(Guid batchId) { _batchId = batchId; return this; }
    public NotificationBuilder WithTemplate(NotificationTemplate template) { _template = template; return this; }
    public NotificationBuilder WithRecipients(List<NotificationRecipient> recipients) { _recipients = recipients; return this; }
    public NotificationBuilder WithAttachments(List<NotificationAttachment> attachments) { _attachments = attachments; return this; }

    public NotificationBuilder AddRecipient(string destination, NotificationChannelType channel)
    {
        _recipients.Add(new NotificationRecipient
        {
            Id = Guid.NewGuid(),
            DestinationAddress = destination,
            ChannelType = channel,
            CreatedAt = DateTime.UtcNow
        });
        return this;
    }

    public Notification Build() => new()
    {
        Id = _id,
        ChannelId = _channelId,
        ProviderId = _providerId,
        Priority = _priority,
        Status = _status,
        Subject = _subject,
        Body = _body,
        TemplateId = _templateId,
        CampaignId = _campaignId,
        BatchId = _batchId,
        Template = _template,
        Recipients = _recipients,
        Attachments = _attachments,
        CreatedAt = DateTime.UtcNow
    };
}

public class NotificationTemplateBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "TestTemplate";
    private NotificationChannelType _channelType = NotificationChannelType.Email;
    private string _subjectTemplate = "Hello {{name}}";
    private string _bodyTemplate = "Welcome {{name}}!";

    public NotificationTemplateBuilder WithId(Guid id) { _id = id; return this; }
    public NotificationTemplateBuilder WithName(string name) { _name = name; return this; }
    public NotificationTemplateBuilder WithChannel(NotificationChannelType channel) { _channelType = channel; return this; }
    public NotificationTemplateBuilder WithSubject(string subject) { _subjectTemplate = subject; return this; }
    public NotificationTemplateBuilder WithBody(string body) { _bodyTemplate = body; return this; }

    public NotificationTemplate Build() => new()
    {
        Id = _id,
        Name = _name,
        ChannelType = _channelType,
        SubjectTemplate = _subjectTemplate,
        BodyTemplate = _bodyTemplate,
        IsActive = true,
        CurrentVersion = 1,
        CreatedAt = DateTime.UtcNow
    };
}

public class NotificationCampaignBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "TestCampaign";
    private NotificationChannelType _channelType = NotificationChannelType.Email;
    private NotificationStatus _status = NotificationStatus.Draft;

    public NotificationCampaignBuilder WithId(Guid id) { _id = id; return this; }
    public NotificationCampaignBuilder WithName(string name) { _name = name; return this; }
    public NotificationCampaignBuilder WithChannel(NotificationChannelType channel) { _channelType = channel; return this; }
    public NotificationCampaignBuilder WithStatus(NotificationStatus status) { _status = status; return this; }

    public NotificationCampaign Build() => new()
    {
        Id = _id,
        Name = _name,
        ChannelType = _channelType,
        Status = _status,
        CreatedAt = DateTime.UtcNow
    };
}

public class NotificationPreferenceBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();
    private NotificationChannelType _channelType = NotificationChannelType.Email;
    private bool _isEnabled = true;
    private TimeOnly? _quietHoursStart;
    private TimeOnly? _quietHoursEnd;
    private int? _maxPerDay;

    public NotificationPreferenceBuilder WithId(Guid id) { _id = id; return this; }
    public NotificationPreferenceBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public NotificationPreferenceBuilder WithChannel(NotificationChannelType channel) { _channelType = channel; return this; }
    public NotificationPreferenceBuilder WithIsEnabled(bool enabled) { _isEnabled = enabled; return this; }
    public NotificationPreferenceBuilder WithQuietHours(TimeOnly? start, TimeOnly? end) { _quietHoursStart = start; _quietHoursEnd = end; return this; }
    public NotificationPreferenceBuilder WithMaxPerDay(int? max) { _maxPerDay = max; return this; }

    public NotificationPreference Build() => new()
    {
        Id = _id,
        UserId = _userId,
        ChannelType = _channelType,
        IsEnabled = _isEnabled,
        QuietHoursStart = _quietHoursStart,
        QuietHoursEnd = _quietHoursEnd,
        MaxPerDay = _maxPerDay,
        CreatedAt = DateTime.UtcNow
    };
}

public class NotificationDeliveryBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _notificationId = Guid.NewGuid();
    private NotificationChannelType _channelType = NotificationChannelType.Email;
    private NotificationStatus _status = NotificationStatus.Queued;
    private string? _providerMessageId = "prov_msg_001";
    private int _attemptCount;

    public NotificationDeliveryBuilder WithId(Guid id) { _id = id; return this; }
    public NotificationDeliveryBuilder WithNotificationId(Guid notificationId) { _notificationId = notificationId; return this; }
    public NotificationDeliveryBuilder WithChannel(NotificationChannelType channel) { _channelType = channel; return this; }
    public NotificationDeliveryBuilder WithStatus(NotificationStatus status) { _status = status; return this; }
    public NotificationDeliveryBuilder WithProviderMessageId(string? msgId) { _providerMessageId = msgId; return this; }
    public NotificationDeliveryBuilder WithAttemptCount(int count) { _attemptCount = count; return this; }

    public NotificationDelivery Build() => new()
    {
        Id = _id,
        NotificationId = _notificationId,
        ChannelType = _channelType,
        Status = _status,
        ProviderMessageId = _providerMessageId,
        AttemptCount = _attemptCount,
        CreatedAt = DateTime.UtcNow
    };
}
