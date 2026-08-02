using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AIService : IAIService
{
    public Task<Result<MessageDto>> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<MessageDto>.Failure("AI message sending not yet implemented"));
    }

    public Task<Result<string>> GetCompletionAsync(GetCompletionRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<string>.Failure("AI completion not yet implemented"));
    }

    public async IAsyncEnumerable<string> GetStreamingCompletionAsync(GetCompletionRequest request, CancellationToken cancellationToken = default)
    {
        yield break;
    }
}
