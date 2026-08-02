using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.AI.ToolCalling;

public interface IToolAuthorizationService
{
    Task<Result<bool>> AuthorizeAsync(Guid toolId, string? userId, string? role, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsToolAllowedForUserAsync(Guid toolId, string userId, CancellationToken cancellationToken = default);
}
