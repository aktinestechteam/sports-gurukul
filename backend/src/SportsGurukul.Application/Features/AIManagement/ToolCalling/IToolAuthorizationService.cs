using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public interface IToolAuthorizationService
{
    Task<Result<bool>> AuthorizeAsync(
        ToolDescriptor tool,
        ToolCallContext context,
        CancellationToken cancellationToken = default);
}
