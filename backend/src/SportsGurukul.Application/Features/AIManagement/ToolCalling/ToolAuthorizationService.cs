using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public class ToolAuthorizationService : IToolAuthorizationService
{
    public Task<Result<bool>> AuthorizeAsync(
        ToolDescriptor tool,
        ToolCallContext context,
        CancellationToken cancellationToken = default)
    {
        if (tool.RequiresApproval)
            return Task.FromResult(Result<bool>.Failure("Tool requires approval before execution"));

        if (tool.IsSystemTool)
            return Task.FromResult(Result<bool>.Success(true));

        if (context.UserId is null)
            return Task.FromResult(Result<bool>.Failure("An authenticated user is required to execute this tool"));

        return Task.FromResult(Result<bool>.Success(true));
    }
}
