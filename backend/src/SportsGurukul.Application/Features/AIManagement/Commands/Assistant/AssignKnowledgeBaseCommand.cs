using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public record AssignKnowledgeBaseCommand(
    Guid AssistantId,
    List<Guid> KnowledgeBaseIds,
    bool ClearExisting
) : IRequest<Result<AssistantDto>>;
