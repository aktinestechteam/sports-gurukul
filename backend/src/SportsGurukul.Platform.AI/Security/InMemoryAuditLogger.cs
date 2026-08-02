using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Security;

public class InMemoryAuditLogger : IAuditLogger
{
    private const int MaxRecords = 10_000;

    private readonly ConcurrentQueue<AuditRecord> _records = new();
    private readonly ILogger<InMemoryAuditLogger> _logger;

    public InMemoryAuditLogger(ILogger<InMemoryAuditLogger>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryAuditLogger>.Instance;
    }

    public Task AuditAsync(
        string action,
        string? entityType,
        string? entityId,
        string? actor,
        string? tenantId,
        string? severity,
        string? details,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _records.Enqueue(new AuditRecord
        {
            Action = action,
            Actor = actor,
            TenantId = tenantId,
            EntityType = entityType,
            EntityId = entityId,
            Severity = severity ?? "Info",
            Details = details,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });

        while (_records.Count > MaxRecords && _records.TryDequeue(out _))
        {
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AuditRecord>> GetAsync(AuditQuery query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var results = _records
            .Where(r => query.TenantId is null || r.TenantId == query.TenantId)
            .Where(r => query.Action is null || r.Action.Equals(query.Action, StringComparison.OrdinalIgnoreCase))
            .Where(r => query.EntityType is null || r.EntityType?.Equals(query.EntityType, StringComparison.OrdinalIgnoreCase) == true)
            .Where(r => query.From is null || r.Timestamp >= query.From)
            .Where(r => query.To is null || r.Timestamp <= query.To)
            .OrderByDescending(r => r.Timestamp)
            .Take(query.Limit > 0 ? query.Limit : 100)
            .ToList();

        return Task.FromResult<IReadOnlyList<AuditRecord>>(results);
    }
}
