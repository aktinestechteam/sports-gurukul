using MediatR;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Events;

public abstract record AiDomainEvent(string? CorrelationId) : INotification
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public sealed record AgentRegisteredEvent(
    string AgentId,
    string Name,
    AgentRole Role,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record AgentRunStartedEvent(
    Guid RunId,
    string AgentId,
    string Goal,
    string? SessionId,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record AgentRunCompletedEvent(
    Guid RunId,
    string AgentId,
    bool Succeeded,
    long DurationMs,
    string? Answer,
    string? Error,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record ToolCallCompletedEvent(
    string ToolName,
    string? AgentId,
    bool Succeeded,
    bool Denied,
    long LatencyMs,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record WorkflowStartedEvent(
    Guid ExecutionId,
    string WorkflowName,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record WorkflowCompletedEvent(
    Guid ExecutionId,
    string WorkflowName,
    bool Succeeded,
    long DurationMs,
    string? Error,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record MemoryWrittenEvent(
    Guid EntryId,
    MemoryCategory Category,
    string Subject,
    string? SessionId,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record ApprovalRequestedEvent(
    Guid RequestId,
    ApprovalType Type,
    string Title,
    string? RunId,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);

public sealed record ApprovalResolvedEvent(
    Guid RequestId,
    ApprovalStatus Status,
    bool Approved,
    string? DecidedBy,
    string? Reason,
    string? TenantId,
    string? CorrelationId) : AiDomainEvent(CorrelationId);
