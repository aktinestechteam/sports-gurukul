using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowService(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<WorkflowDto>> GetByIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken);
        if (workflow is null)
            return Result<WorkflowDto>.Failure("Workflow not found");

        return Result<WorkflowDto>.Success(MapToDto(workflow));
    }

    public async Task<Result<IReadOnlyList<WorkflowDto>>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var workflows = await _workflowRepository.GetPublishedAsync(cancellationToken);
        return Result<IReadOnlyList<WorkflowDto>>.Success(workflows.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<WorkflowDto>>> SearchAsync(
        string? searchTerm,
        AIWorkflowType? workflowType,
        bool? isActive,
        bool? isPublished,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WorkflowDefinition> workflows;
        if (workflowType.HasValue)
            workflows = await _workflowRepository.GetByTypeAsync(workflowType.Value, cancellationToken);
        else if (isPublished == true)
            workflows = await _workflowRepository.GetPublishedAsync(cancellationToken);
        else if (isActive.HasValue)
            workflows = await _workflowRepository.GetActiveAsync(cancellationToken);
        else
            workflows = await _workflowRepository.GetAllAsync(cancellationToken);

        var query = workflows.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(w =>
                w.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (w.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (isPublished.HasValue)
            query = query.Where(w => w.IsPublished == isPublished.Value);
        if (isActive.HasValue)
            query = query.Where(w => w.IsActive == isActive.Value);

        var paged = query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<WorkflowDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static WorkflowDto MapToDto(WorkflowDefinition workflow)
        => new(
            workflow.Id,
            workflow.Name,
            workflow.Description,
            workflow.WorkflowType,
            workflow.DefinitionJson,
            workflow.EntryNode,
            workflow.Version,
            workflow.IsActive,
            workflow.IsPublished,
            workflow.TimeoutSeconds,
            workflow.CreatedAt,
            workflow.UpdatedAt);
}
