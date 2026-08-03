namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public interface IToolExecutor
{
    Task<ToolCallResult> ExecuteAsync(
        string toolName,
        ToolCallRequest request,
        CancellationToken cancellationToken = default);
}
