using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AuditService : IAuditService
{
    private readonly IAIAuditLogRepository _auditRepository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAIAuditLogRepository auditRepository,
        ILogger<AuditService> logger)
    {
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<Result<AIAuditLog>> RecordAsync(RecordAuditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new AIAuditLog
        {
            Id = Guid.NewGuid(),
            EntityId = request.EntityId,
            EntityType = request.EntityType,
            EventType = request.EventType,
            Severity = request.Severity,
            Action = request.Action,
            ActorId = request.ActorId,
            ActorType = request.ActorType,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            PreviousState = request.PreviousState,
            NewState = request.NewState,
            Message = request.Message,
            Metadata = request.Metadata,
            CreatedAt = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Recorded audit log {AuditLogId} for entity {EntityType}/{EntityId}", entity.Id, entity.EntityType, entity.EntityId);

        return Result<AIAuditLog>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<AIAuditLog>>> GetByEntityAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default)
    {
        var result = await _auditRepository.FindAsync(a =>
            a.EntityId == entityId && a.EntityType == entityType, cancellationToken);

        return Result<IReadOnlyList<AIAuditLog>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AIAuditLog>>> GetByEventTypeAsync(AuditEventType eventType, CancellationToken cancellationToken = default)
    {
        var result = await _auditRepository.GetByEventTypeAsync(eventType.ToString(), cancellationToken);

        return Result<IReadOnlyList<AIAuditLog>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AIAuditLog>>> GetRecentBySeverityAsync(AuditSeverity severity, int take, CancellationToken cancellationToken = default)
    {
        var result = await _auditRepository.GetRecentBySeverityAsync(severity.ToString(), take, cancellationToken);

        return Result<IReadOnlyList<AIAuditLog>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AIAuditLog>>> SearchAsync(SearchAuditRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _auditRepository.FindAsync(a =>
            (string.IsNullOrEmpty(request.EntityType) || a.EntityType == request.EntityType) &&
            (!request.EntityId.HasValue || a.EntityId == request.EntityId) &&
            (!request.EventType.HasValue || a.EventType == request.EventType) &&
            (!request.Severity.HasValue || a.Severity == request.Severity) &&
            (!request.FromDate.HasValue || a.CreatedAt >= request.FromDate.Value) &&
            (!request.ToDate.HasValue || a.CreatedAt <= request.ToDate.Value), cancellationToken);

        return Result<IReadOnlyList<AIAuditLog>>.Success(result);
    }
}
