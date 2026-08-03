using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IRepository<AIModel> _modelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IAgentRepository agentRepository,
        IWorkflowRepository workflowRepository,
        IRepository<AIModel> modelRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<AgentService> logger)
    {
        _agentRepository = agentRepository;
        _workflowRepository = workflowRepository;
        _modelRepository = modelRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<AgentDto>> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.WorkflowId.HasValue)
        {
            var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId.Value, cancellationToken);
            if (workflow is null)
                return Result<AgentDto>.Failure("Referenced workflow does not exist");
        }

        if (request.ModelId.HasValue)
        {
            var model = await _modelRepository.GetByIdAsync(request.ModelId.Value, cancellationToken);
            if (model is null || !model.IsActive)
                return Result<AgentDto>.Failure("Referenced model does not exist or is not active");
        }

        var agent = new AgentDefinition
        {
            WorkflowId = request.WorkflowId,
            ModelId = request.ModelId,
            Name = request.Name,
            Description = request.Description,
            AgentType = request.AgentType,
            SystemPrompt = request.SystemPrompt,
            Temperature = request.Temperature,
            MaxIterations = request.MaxIterations,
            MemoryEnabled = request.MemoryEnabled,
            IsActive = true,
            ToolsJson = request.ToolsJson,
            MetadataJson = request.MetadataJson,
        };

        await _agentRepository.AddAsync(agent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new AgentCreatedEvent(agent.Id, agent.Name, agent.AgentType, DateTime.UtcNow),
            cancellationToken);

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<AgentDto>> UpdateAsync(UpdateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken);
        if (agent is null)
            return Result<AgentDto>.Failure("Agent not found");

        if (request.ExpectedRowVersion is { Length: > 0 } && !agent.RowVersion.SequenceEqual(request.ExpectedRowVersion))
            return Result<AgentDto>.Failure("The agent was modified by another user; please refresh and try again");

        if (!string.IsNullOrWhiteSpace(request.Name)) agent.Name = request.Name;
        if (request.Description is not null) agent.Description = request.Description;
        if (request.AgentType.HasValue) agent.AgentType = request.AgentType.Value;
        if (request.SystemPrompt is not null) agent.SystemPrompt = request.SystemPrompt;
        if (request.Temperature.HasValue) agent.Temperature = request.Temperature;
        if (request.MaxIterations.HasValue) agent.MaxIterations = request.MaxIterations;
        if (request.MemoryEnabled.HasValue) agent.MemoryEnabled = request.MemoryEnabled.Value;
        if (request.ModelId.HasValue) agent.ModelId = request.ModelId;
        if (request.ToolsJson is not null) agent.ToolsJson = request.ToolsJson;

        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepository.Update(agent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<AgentDto>> EnableAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            return Result<AgentDto>.Failure("Agent not found");

        agent.IsActive = true;
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepository.Update(agent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<AgentDto>> DisableAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            return Result<AgentDto>.Failure("Agent not found");

        agent.IsActive = false;
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepository.Update(agent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<AgentDto>> AssignWorkflowAsync(Guid agentId, Guid workflowId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            return Result<AgentDto>.Failure("Agent not found");

        var workflow = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken);
        if (workflow is null)
            return Result<AgentDto>.Failure("Workflow not found");

        agent.WorkflowId = workflowId;
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepository.Update(agent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new WorkflowAssignedEvent(agent.Id, workflowId, DateTime.UtcNow),
            cancellationToken);

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<AgentDto>> GetByIdAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdWithToolsAsync(agentId, cancellationToken);
        if (agent is null)
            return Result<AgentDto>.Failure("Agent not found");

        return Result<AgentDto>.Success(MapToDto(agent));
    }

    public async Task<Result<IReadOnlyList<AgentDto>>> SearchAsync(
        string? searchTerm,
        AIAgentType? agentType,
        Guid? workflowId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AgentDefinition> agents;
        if (workflowId.HasValue)
            agents = await _agentRepository.GetByWorkflowAsync(workflowId.Value, cancellationToken);
        else if (agentType.HasValue)
            agents = await _agentRepository.GetByTypeAsync(agentType.Value, cancellationToken);
        else
            agents = await _agentRepository.GetAllAsync(cancellationToken);

        var query = agents.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (a.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        var paged = query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<AgentDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static AgentDto MapToDto(AgentDefinition agent)
    {
        var tools = (agent.Tools ?? new List<ToolDefinition>())
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(MapToToolDto)
            .ToList();

        return new AgentDto(
            agent.Id,
            agent.WorkflowId,
            agent.ModelId,
            agent.Name,
            agent.Description,
            agent.AgentType,
            agent.SystemPrompt,
            agent.Temperature,
            agent.MaxIterations,
            agent.MemoryEnabled,
            agent.IsActive,
            tools,
            agent.CreatedAt,
            agent.UpdatedAt);
    }

    private static ToolDto MapToToolDto(ToolDefinition tool)
        => new(
            tool.Id,
            tool.AgentId,
            tool.Name,
            tool.Description,
            tool.ToolType,
            tool.Endpoint,
            tool.HttpMethod,
            tool.InputSchemaJson,
            tool.OutputSchemaJson,
            tool.IsActive,
            tool.IsSystemTool,
            tool.TimeoutSeconds,
            tool.RequiresApproval,
            tool.RetryPolicyJson);
}
