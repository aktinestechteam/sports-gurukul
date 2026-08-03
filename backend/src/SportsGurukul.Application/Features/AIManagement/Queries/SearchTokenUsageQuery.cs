using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record SearchTokenUsageQuery(
    Guid? AssistantId,
    Guid? ConversationId,
    Guid? UserId,
    AIUsageType? UsageType,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyList<TokenUsageDto>>>;
