using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAthleteDocumentRepository : IRepository<AthleteDocument>
{
    Task<IReadOnlyList<AthleteDocument>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<AthleteDocument?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentVersion>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentAudit>> GetAuditTrailAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<int> GetMaxVersionNumberAsync(Guid documentId, CancellationToken cancellationToken = default);
}
