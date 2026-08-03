using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record GetTokenUsageSummaryQuery(
    Guid? AssistantId,
    Guid? ConversationId,
    Guid? UserId,
    DateTime? From,
    DateTime? To,
    AIUsageType? UsageType
) : IRequest<Result<TokenUsageSummaryDto>>;
