using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork,
        ILogger<AuditService> logger)
    {
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AuditLogDto>> WriteAsync(WriteAuditRequest request, CancellationToken cancellationToken = default)
    {
        var log = new AIAuditLog
        {
            ActorUserId = request.ActorUserId,
            ActorType = request.ActorType,
            Action = request.Action,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            DetailsJson = request.DetailsJson,
            BeforeJson = request.BeforeJson,
            AfterJson = request.AfterJson,
            ChangedFieldsJson = request.ChangedFieldsJson,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            CorrelationId = request.CorrelationId,
            Severity = request.Severity,
        };

        await _auditRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AI audit entry recorded: {Action} on {EntityType} {EntityId}", log.Action, log.EntityType, log.EntityId);
        return Result<AuditLogDto>.Success(MapToDto(log));
    }

    public async Task<Result<IReadOnlyList<AuditLogDto>>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.GetByEntityAsync(entityType, entityId, cancellationToken);
        return Result<IReadOnlyList<AuditLogDto>>.Success(logs.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<AuditLogDto>>> GetByActionAsync(AIAuditAction action, CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.GetByActionAsync(action, cancellationToken);
        return Result<IReadOnlyList<AuditLogDto>>.Success(logs.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<AuditLogDto>>> GetByActorAsync(Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.GetByActorAsync(actorUserId, cancellationToken);
        return Result<IReadOnlyList<AuditLogDto>>.Success(logs.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<AuditLogDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.GetByDateRangeAsync(from, to, cancellationToken);
        return Result<IReadOnlyList<AuditLogDto>>.Success(logs.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<AuditLogDto>>> SearchAsync(
        string? entityType,
        Guid? entityId,
        AIAuditAction? action,
        Guid? actorUserId,
        AIAuditSeverity? severity,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.FindAsync(
            l =>
                (string.IsNullOrWhiteSpace(entityType) || l.EntityType == entityType) &&
                (!entityId.HasValue || l.EntityId == entityId.Value) &&
                (!action.HasValue || l.Action == action.Value) &&
                (!actorUserId.HasValue || l.ActorUserId == actorUserId.Value) &&
                (!severity.HasValue || l.Severity == severity.Value) &&
                (!from.HasValue || l.CreatedAt >= from.Value) &&
                (!to.HasValue || l.CreatedAt <= to.Value),
            cancellationToken);

        var paged = logs
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<AuditLogDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static AuditLogDto MapToDto(AIAuditLog log)
        => new(
            log.Id,
            log.ActorUserId,
            log.ActorType,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.DetailsJson,
            log.BeforeJson,
            log.AfterJson,
            log.ChangedFieldsJson,
            log.IpAddress,
            log.UserAgent,
            log.CorrelationId,
            log.Severity,
            log.CreatedAt);
}
