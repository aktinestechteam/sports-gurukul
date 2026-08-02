using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record CreateConversationCommand(
    string? Title,
    Guid? AssistantId,
    Guid? UserId
) : IRequest<Result<ConversationDto>>;
