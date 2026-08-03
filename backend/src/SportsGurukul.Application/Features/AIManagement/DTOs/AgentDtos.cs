using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record AgentDto(
    Guid Id,
    Guid? WorkflowId,
    Guid? ModelId,
    string Name,
    string? Description,
    AIAgentType AgentType,
    string? SystemPrompt,
    double? Temperature,
    int? MaxIterations,
    bool MemoryEnabled,
    bool IsActive,
    List<ToolDto> Tools,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record WorkflowDto(
    Guid Id,
    string Name,
    string? Description,
    AIWorkflowType WorkflowType,
    string DefinitionJson,
    string? EntryNode,
    int Version,
    bool IsActive,
    bool IsPublished,
    int? TimeoutSeconds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record ToolDto(
    Guid Id,
    Guid AgentId,
    string Name,
    string? Description,
    AIToolType ToolType,
    string? Endpoint,
    string? HttpMethod,
    string InputSchemaJson,
    string? OutputSchemaJson,
    bool IsActive,
    bool IsSystemTool,
    int? TimeoutSeconds,
    bool RequiresApproval,
    string? RetryPolicyJson
);

public record CreateAgentRequest(
    Guid? WorkflowId,
    Guid? ModelId,
    string Name,
    string? Description,
    AIAgentType AgentType,
    string? SystemPrompt,
    double? Temperature,
    int? MaxIterations,
    bool MemoryEnabled,
    string? ToolsJson,
    string? MetadataJson
);

public record UpdateAgentRequest(
    Guid AgentId,
    string? Name,
    string? Description,
    AIAgentType? AgentType,
    string? SystemPrompt,
    double? Temperature,
    int? MaxIterations,
    bool? MemoryEnabled,
    Guid? ModelId,
    string? ToolsJson,
    byte[]? ExpectedRowVersion
);
