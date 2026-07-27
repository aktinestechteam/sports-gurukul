using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAssessmentRepository : IRepository<TrainingAssessment>
{
    Task<TrainingAssessment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingAssessment>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingAssessment>> GetByTypeAsync(AssessmentType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssessmentResult>> GetResultsByAssessmentIdAsync(Guid assessmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssessmentResult>> GetResultsByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
}
