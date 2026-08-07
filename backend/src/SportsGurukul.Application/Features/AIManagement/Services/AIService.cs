using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AIService : IAIService
{
    private readonly IConversationService _conversationService;
    private readonly IModelRoutingService _modelRoutingService;
    private readonly ITokenUsageService _tokenUsageService;

    public AIService(
        IConversationService conversationService,
        IModelRoutingService modelRoutingService,
        ITokenUsageService tokenUsageService)
    {
        _conversationService = conversationService;
        _modelRoutingService = modelRoutingService;
        _tokenUsageService = tokenUsageService;
    }

    public Task<Result<ConversationDto>> CreateConversationAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
        => _conversationService.CreateAsync(request, cancellationToken);

    public Task<Result<MessageDto>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default)
        => _conversationService.AddMessageAsync(request, cancellationToken);

    public Task<Result<ModelSelectionResult>> SelectModelAsync(ModelSelectionContext context, CancellationToken cancellationToken = default)
        => _modelRoutingService.SelectModelAsync(context, cancellationToken);

    public Task<Result<TokenUsageDto>> RecordTokenUsageAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default)
        => _tokenUsageService.RecordAsync(request, cancellationToken);
}
