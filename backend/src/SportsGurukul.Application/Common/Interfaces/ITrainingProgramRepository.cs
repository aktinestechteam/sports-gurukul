using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ITrainingProgramRepository : IRepository<TrainingProgram>
{
    Task<TrainingProgram?> GetByProgramCodeAsync(string programCode, CancellationToken cancellationToken = default);
    Task<TrainingProgram?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingProgram>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingProgram>> GetBySportIdAsync(Guid sportId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingProgram>> GetByStatusAsync(TrainingProgramStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsProgramCodeUniqueAsync(string programCode, CancellationToken cancellationToken = default);
}
