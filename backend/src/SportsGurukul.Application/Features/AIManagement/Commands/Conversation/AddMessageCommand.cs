using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record AddMessageCommand(
    Guid ConversationId,
    AIMessageRole Role,
    AIMessageContentType ContentType,
    string Content,
    string? ModelName,
    int? PromptVersionUsed,
    int? InputTokenCount,
    int? OutputTokenCount,
    long? LatencyMs,
    string? ToolCallsJson,
    string? ToolResultsJson
) : IRequest<Result<MessageDto>>;
