using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public record ToolDescriptor(
    Guid? ToolDefinitionId,
    string Name,
    AIToolType ToolType,
    string? Description,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    bool IsSystemTool,
    bool RequiresApproval,
    Func<ToolCallRequest, CancellationToken, Task<ToolCallResult>>? Executor
);
