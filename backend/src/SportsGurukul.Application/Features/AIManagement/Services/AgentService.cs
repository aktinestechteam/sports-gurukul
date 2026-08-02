using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AgentService : IAgentService
{
    private readonly IAgentDefinitionRepository _agentRepository;
    private readonly IWorkflowDefinitionRepository _workflowRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IAgentDefinitionRepository agentRepository,
        IWorkflowDefinitionRepository workflowRepository,
        IPublisher publisher,
        ILogger<AgentService> logger)
    {
        _agentRepository = agentRepository;
        _workflowRepository = workflowRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<AgentDefinition>> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            AssistantId = request.AssistantId,
            Configuration = request.Configuration,
            Tools = request.Tools,
            Rules = request.Rules,
            Constraints = request.Constraints,
            MaxIterations = request.MaxIterations ?? 10,
            RequiresApproval = request.RequiresApproval ?? false,
            Status = AgentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _agentRepository.AddAsync(entity, cancellationToken);

        await _publisher.Publish(new AgentCreatedEvent(entity.Id, entity.Name, entity.AssistantId, entity.CreatedAt), cancellationToken);

        _logger.LogInformation("Created agent {AgentId} with name {Name}", entity.Id, entity.Name);

        return Result<AgentDefinition>.Success(entity);
    }

    public async Task<Result<AgentDefinition>> UpdateAsync(UpdateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _agentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AgentDefinition>.Failure("Agent not found");

        if (request.Name is not null) entity.Name = request.Name;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Configuration is not null) entity.Configuration = request.Configuration;
        if (request.Tools is not null) entity.Tools = request.Tools;
        if (request.Rules is not null) entity.Rules = request.Rules;
        if (request.Constraints is not null) entity.Constraints = request.Constraints;
        if (request.MaxIterations.HasValue) entity.MaxIterations = request.MaxIterations.Value;
        if (request.RequiresApproval.HasValue) entity.RequiresApproval = request.RequiresApproval.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _agentRepository.Update(entity);

        _logger.LogInformation("Updated agent {AgentId}", request.Id);

        return Result<AgentDefinition>.Success(entity);
    }

    public async Task<Result<AgentDefinition>> EnableAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _agentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AgentDefinition>.Failure("Agent not found");

        entity.Status = AgentStatus.Active;
        entity.UpdatedAt = DateTime.UtcNow;

        _agentRepository.Update(entity);

        _logger.LogInformation("Enabled agent {AgentId}", id);

        return Result<AgentDefinition>.Success(entity);
    }

    public async Task<Result<AgentDefinition>> DisableAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _agentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AgentDefinition>.Failure("Agent not found");

        entity.Status = AgentStatus.Inactive;
        entity.UpdatedAt = DateTime.UtcNow;

        _agentRepository.Update(entity);

        _logger.LogInformation("Disabled agent {AgentId}", id);

        return Result<AgentDefinition>.Success(entity);
    }

    public async Task<Result<bool>> AssignWorkflowAsync(Guid agentId, Guid workflowId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null || agent.IsDeleted)
            return Result<bool>.Failure("Agent not found");

        var workflow = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken);
        if (workflow is null || workflow.IsDeleted)
            return Result<bool>.Failure("Workflow not found");

        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepository.Update(agent);

        await _publisher.Publish(new WorkflowAssignedEvent(agentId, workflowId, DateTime.UtcNow), cancellationToken);

        _logger.LogInformation("Assigned workflow {WorkflowId} to agent {AgentId}", workflowId, agentId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<AgentDefinition>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _agentRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AgentDefinition>.Failure("Agent not found");

        return Result<AgentDefinition>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<AgentDefinition>>> SearchAsync(SearchAgentsRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _agentRepository.FindAsync(a =>
            !a.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || a.Name.Contains(request.SearchTerm) || (a.Description != null && a.Description.Contains(request.SearchTerm))) &&
            (!request.Status.HasValue || a.Status == request.Status) &&
            (!request.AssistantId.HasValue || a.AssistantId == request.AssistantId), cancellationToken);

        return Result<IReadOnlyList<AgentDefinition>>.Success(query);
    }
}
