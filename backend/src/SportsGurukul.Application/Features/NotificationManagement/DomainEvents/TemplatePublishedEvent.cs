using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record TemplatePublishedEvent(
    Guid TemplateId,
    string Name,
    int VersionNumber,
    DateTime PublishedAt
) : INotification;
