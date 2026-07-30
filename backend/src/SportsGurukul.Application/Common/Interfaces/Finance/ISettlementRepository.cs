using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance;

public interface ISettlementRepository : IRepository<SettlementBatch>
{
    Task<SettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<SettlementBatch?> GetByIdWithSettlementsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Settlement>> GetSettlementsByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
}
