using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Security;

internal sealed class KnowledgeAuditLogger : IKnowledgeAuditLogger
{
    private readonly ConcurrentQueue<KnowledgeAuditEvent> _events = new();
    private readonly int _capacity;
    private readonly ILogger<KnowledgeAuditLogger> _logger;

    public KnowledgeAuditLogger(KnowledgePlatformOptions options, ILogger<KnowledgeAuditLogger> logger)
    {
        _capacity = Math.Max(1, options.Security.AuditBufferSize);
        _logger = logger;
    }

    public Task LogAsync(KnowledgeAuditEvent auditEvent, CancellationToken ct = default)
    {
        if (auditEvent is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "Knowledge audit {Action} tenant={TenantId} index={IndexName} actor={Actor} entity={EntityType}:{EntityId} succeeded={Succeeded} reason={Reason}",
            auditEvent.Action,
            auditEvent.TenantId,
            auditEvent.IndexName,
            auditEvent.ActorUserId,
            auditEvent.EntityType,
            auditEvent.EntityId,
            auditEvent.Succeeded,
            auditEvent.Reason);

        while (_events.Count >= _capacity)
        {
            _events.TryDequeue(out _);
        }

        _events.Enqueue(auditEvent);
        return Task.CompletedTask;
    }

    internal IReadOnlyList<KnowledgeAuditEvent> Snapshot() => _events.ToArray();
}
