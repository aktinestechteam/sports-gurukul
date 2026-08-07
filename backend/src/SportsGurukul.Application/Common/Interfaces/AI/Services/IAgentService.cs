using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAgentService
{
    Task<Result<AgentDto>> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken = default);

    Task<Result<AgentDto>> UpdateAsync(UpdateAgentRequest request, CancellationToken cancellationToken = default);

    Task<Result<AgentDto>> EnableAsync(Guid agentId, CancellationToken cancellationToken = default);

    Task<Result<AgentDto>> DisableAsync(Guid agentId, CancellationToken cancellationToken = default);

    Task<Result<AgentDto>> AssignWorkflowAsync(Guid agentId, Guid workflowId, CancellationToken cancellationToken = default);

    Task<Result<AgentDto>> GetByIdAsync(Guid agentId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AgentDto>>> SearchAsync(
        string? searchTerm,
        AIAgentType? agentType,
        Guid? workflowId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
