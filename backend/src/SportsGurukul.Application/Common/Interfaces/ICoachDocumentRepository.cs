using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachDocumentRepository : IRepository<CoachDocument>
{
    Task<IReadOnlyList<CoachDocument>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<CoachDocument?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachDocumentVersion>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoachDocumentAudit>> GetAuditTrailAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<int> GetMaxVersionNumberAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task AddVersionAsync(CoachDocumentVersion version, CancellationToken cancellationToken = default);
    Task AddAuditAsync(CoachDocumentAudit audit, CancellationToken cancellationToken = default);
}
