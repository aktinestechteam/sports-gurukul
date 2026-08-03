using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record CreateConversationCommand(
    Guid AssistantId,
    string Title,
    AIResourceOwnerType ParticipantType,
    Guid? ParticipantUserId,
    List<Guid>? KnowledgeBaseIds,
    string? ContextMetadataJson
) : IRequest<Result<ConversationDto>>;
