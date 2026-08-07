using MediatR;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Events;

public record MessageAddedEvent(
    Guid ConversationId,
    Guid MessageId,
    Guid AssistantId,
    AIMessageRole Role,
    int SequenceNumber,
    DateTime AddedAt
) : INotification;
