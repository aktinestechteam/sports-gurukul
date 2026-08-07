namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public record ToolCallResult(
    bool IsSuccess,
    string? OutputJson,
    string? ErrorMessage,
    long DurationMs,
    decimal? Cost,
    bool RequiresApproval
)
{
    public static ToolCallResult Success(string? outputJson, long durationMs, decimal? cost = null) =>
        new(true, outputJson, null, durationMs, cost, false);

    public static ToolCallResult Failure(string error, long durationMs) =>
        new(false, null, error, durationMs, null, false);

    public static ToolCallResult ApprovalRequired(long durationMs) =>
        new(false, null, "Tool execution requires approval", durationMs, null, true);
}
