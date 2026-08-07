using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IWorkflowService
{
    Task<Result<WorkflowDto>> GetByIdAsync(Guid workflowId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<WorkflowDto>>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<WorkflowDto>>> SearchAsync(
        string? searchTerm,
        AIWorkflowType? workflowType,
        bool? isActive,
        bool? isPublished,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
