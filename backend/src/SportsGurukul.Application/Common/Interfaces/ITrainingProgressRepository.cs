using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ITrainingProgressRepository : IRepository<TrainingProgress>
{
    Task<TrainingProgress?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<TrainingProgress?> GetByEnrollmentIdWithDetailsAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingProgress>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default);
}
