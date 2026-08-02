using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface IToolAuthorization
{
    Task<ToolAuthorizationDecision> AuthorizeAsync(ITool tool, ToolExecutionContext context, CancellationToken cancellationToken = default);
}
