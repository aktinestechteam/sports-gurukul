using MediatR;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record PromptPublishedEvent(
    Guid PromptTemplateId,
    Guid AssistantId,
    string Name,
    int VersionNumber,
    DateTime PublishedAt
) : INotification;
