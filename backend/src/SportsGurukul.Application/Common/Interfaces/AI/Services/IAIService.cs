using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAIService
{
    Task<Result<MessageDto>> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> GetCompletionAsync(GetCompletionRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetStreamingCompletionAsync(GetCompletionRequest request, CancellationToken cancellationToken = default);
}
