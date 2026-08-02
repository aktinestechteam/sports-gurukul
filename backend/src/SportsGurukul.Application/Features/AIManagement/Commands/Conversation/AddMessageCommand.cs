using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record AddMessageCommand(
    Guid ConversationId,
    MessageRole Role,
    string Content,
    string? Metadata
) : IRequest<Result<ConversationDto>>;
