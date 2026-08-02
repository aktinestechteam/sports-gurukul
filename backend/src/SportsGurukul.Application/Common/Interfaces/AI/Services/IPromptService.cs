using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IPromptService
{
    Task<Result<PromptTemplate>> CreateAsync(CreatePromptTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<PromptTemplate>> UpdateAsync(UpdatePromptTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<PromptTemplate>> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PromptTemplate>> RollbackAsync(Guid id, int versionNumber, CancellationToken cancellationToken = default);
    Task<Result<PromptTemplate>> CloneAsync(Guid id, string newName, CancellationToken cancellationToken = default);
    Task<Result<PromptTemplate>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<PromptVersion>>> GetVersionsAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<PromptTemplate>>> SearchAsync(SearchPromptsRequest request, CancellationToken cancellationToken = default);
}
