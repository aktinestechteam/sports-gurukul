using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ITrainingBatchRepository : IRepository<TrainingBatch>
{
    Task<TrainingBatch?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default);
    Task<TrainingBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingBatch>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingBatch>> GetByCoachIdAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingBatch>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainingBatch>> GetByStatusAsync(BatchStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsBatchCodeUniqueAsync(string batchCode, CancellationToken cancellationToken = default);
}
