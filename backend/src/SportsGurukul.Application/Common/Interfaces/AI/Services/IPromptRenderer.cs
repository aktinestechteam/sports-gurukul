using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IPromptRenderer
{
    Result<string> Render(string template, IReadOnlyDictionary<string, string> variables);

    Task<Result<string>> ResolveAndRenderAsync(
        Guid assistantId,
        AIPromptType promptType,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default);
}
