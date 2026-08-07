namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public record ToolCallRequest(
    string ToolName,
    string? InputJson,
    ToolCallContext Context
);
