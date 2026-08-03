using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAssistantService
{
    Task<Result<AssistantDto>> CreateAsync(CreateAssistantRequest request, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> UpdateAsync(UpdateAssistantRequest request, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> PublishAsync(Guid assistantId, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> ArchiveAsync(Guid assistantId, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> AssignKnowledgeBaseAsync(AssignKnowledgeBaseRequest request, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> AssignToolsAsync(AssignToolsRequest request, CancellationToken cancellationToken = default);

    Task<Result<AssistantDto>> GetByIdAsync(Guid assistantId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AssistantDto>>> SearchAsync(
        string? searchTerm,
        AIAssistantType? assistantType,
        Guid? ownerUserId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
