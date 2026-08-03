using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IPromptService
{
    Task<Result<PromptTemplateDto>> CreateAsync(CreatePromptTemplateRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromptTemplateDto>> UpdateAsync(UpdatePromptTemplateRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromptTemplateDto>> PublishAsync(PublishPromptTemplateRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromptTemplateDto>> RollbackAsync(RollbackPromptVersionRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromptTemplateDto>> CloneAsync(ClonePromptRequest request, CancellationToken cancellationToken = default);

    Task<Result<PromptTemplateDto>> GetByIdAsync(Guid promptTemplateId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PromptTemplateDto>>> SearchAsync(
        string? searchTerm,
        Guid? assistantId,
        AIPromptType? promptType,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
