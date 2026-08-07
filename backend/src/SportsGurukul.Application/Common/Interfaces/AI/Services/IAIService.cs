using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAIService
{
    Task<Result<ConversationDto>> CreateConversationAsync(CreateConversationRequest request, CancellationToken cancellationToken = default);

    Task<Result<MessageDto>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default);

    Task<Result<ModelSelectionResult>> SelectModelAsync(ModelSelectionContext context, CancellationToken cancellationToken = default);

    Task<Result<TokenUsageDto>> RecordTokenUsageAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default);
}
