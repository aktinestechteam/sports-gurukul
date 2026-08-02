using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public record AssignToolsCommand(
    Guid AssistantId,
    List<Guid> ToolIds
) : IRequest<Result<AssistantDto>>;
