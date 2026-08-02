using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.DomainEvents;

public record PromptPublishedEvent(
    Guid PromptTemplateId,
    string Name,
    int VersionNumber,
    DateTime PublishedAt
) : INotification;
