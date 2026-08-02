using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowDefinitionRepository _workflowRepository;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(
        IWorkflowDefinitionRepository workflowRepository,
        ILogger<WorkflowService> logger)
    {
        _workflowRepository = workflowRepository;
        _logger = logger;
    }

    public async Task<Result<WorkflowDefinition>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _workflowRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<WorkflowDefinition>.Failure("Workflow not found");

        return Result<WorkflowDefinition>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<WorkflowDefinition>>> SearchAsync(SearchWorkflowsRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _workflowRepository.FindAsync(w =>
            !w.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || w.Name.Contains(request.SearchTerm) || (w.Description != null && w.Description.Contains(request.SearchTerm))) &&
            (!request.Status.HasValue || w.Status == request.Status), cancellationToken);

        _logger.LogInformation("Searched workflows with term {SearchTerm}, found {Count} results", request.SearchTerm, query.Count);

        return Result<IReadOnlyList<WorkflowDefinition>>.Success(query);
    }
}
