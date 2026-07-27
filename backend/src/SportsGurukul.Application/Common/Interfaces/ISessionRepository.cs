using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ISessionRepository : IRepository<TrainingSession>
{
    Task<TrainingSession?> GetBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken = default);
    Task<TrainingSession?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingSession>> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingSession>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingSession>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingSession>> GetBySessionDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingSession>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsSessionCodeUniqueAsync(string sessionCode, CancellationToken cancellationToken = default);
}
