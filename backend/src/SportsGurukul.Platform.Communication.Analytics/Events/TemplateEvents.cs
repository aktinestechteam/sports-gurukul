using MediatR;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Analytics.Events;

public record TemplateCreatedEvent(
    Guid TemplateId,
    string Name,
    string? CreatedBy,
    DateTime CreatedAt
) : INotification;

public record TemplatePublishedEvent(
    Guid TemplateId,
    int VersionNumber,
    string? PublishedBy,
    DateTime PublishedAt
) : INotification;

public record TemplateArchivedEvent(
    Guid TemplateId,
    DateTime ArchivedAt
) : INotification;

public record TemplateRolledBackEvent(
    Guid TemplateId,
    int PreviousVersion,
    int NewVersion,
    string? ChangeNotes
) : INotification;

public class TemplatePublishedEventHandler(ILogger<TemplatePublishedEventHandler> logger)
    : INotificationHandler<TemplatePublishedEvent>
{
    public async Task Handle(TemplatePublishedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Template {TemplateId} (v{VersionNumber}) published by {PublishedBy} at {PublishedAt}",
            notification.TemplateId,
            notification.VersionNumber,
            notification.PublishedBy,
            notification.PublishedAt);

        await Task.CompletedTask;
    }
}
