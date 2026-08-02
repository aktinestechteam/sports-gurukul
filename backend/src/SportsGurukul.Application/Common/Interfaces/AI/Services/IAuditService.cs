using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAuditService
{
    Task<Result<AIAuditLog>> RecordAsync(RecordAuditRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIAuditLog>>> GetByEntityAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIAuditLog>>> GetByEventTypeAsync(AuditEventType eventType, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIAuditLog>>> GetRecentBySeverityAsync(AuditSeverity severity, int take, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIAuditLog>>> SearchAsync(SearchAuditRequest request, CancellationToken cancellationToken = default);
}
